using AACS.Service.Interface;
using DataAccess.Models;
using Core.Interfaces;
using Infrastructure.DomainService;
using MediatR;
using Core.Notifications;
using Core.Bus;
using Microsoft.AspNetCore.Identity;
using System.Models;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using AACS.Repository.Interface;
using System.Globalization;
using System.Storage;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;

namespace AACS.Service.Implements
{
    public class FaceDetectionService : DomainService, IFaceDetectionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserEmbeddingRepo _userEmbeddingRepo;
        private readonly IStorageService _storageService;
        private readonly IAttendanceRepo _attendanceRepo;
        private readonly IMediatorHandler _bus;
        private readonly ISubjectScheduleStudentRepo _subjectScheduleStudentRepo;
        private readonly IDistributedCache _cache;
        private const string EmbeddingCacheKey = "UserEmbeddings";
        public FaceDetectionService(
             UserManager<ApplicationUser> userManager,
            IStorageService storageService,
            INotificationHandler<DomainNotification> notifications,
            IAttendanceRepo attendanceRepo,
            IUserEmbeddingRepo userEmbeddingRepo,
            ISubjectScheduleStudentRepo subjectScheduleStudentRepo,
            IMapper mapper,
            IUnitOfWork uow,
            IDistributedCache cache,
            IMediatorHandler bus
        ) : base(notifications, uow, bus)
        {
            _userEmbeddingRepo = userEmbeddingRepo;
            _userManager = userManager;
            _mapper = mapper;
            _storageService = storageService;
            _attendanceRepo = attendanceRepo;
            _subjectScheduleStudentRepo = subjectScheduleStudentRepo;
            _bus = bus;
            _cache = cache;
        }


        public UserRes? FaceRecognitionAsync(string base64Img, string subjectScheduleDetailId)
        {
            // Turn base64 image to a file and upload to storage
            var imageFile = ConvertBase64ToFormFile(base64Img);
            var imageUrl = _storageService.UploadObjectAsync(imageFile, default).Result;

            // Recognize user from the image
            var userId = RecognizeUser(base64Img);
            var attendance = new Attendance
            {
                UserId = userId,
                AttendanceId = Guid.NewGuid().ToString(),
                AttendanceTime = DateTime.UtcNow,
                ImageUrl = imageUrl,
                SubjectScheduleDetailId = subjectScheduleDetailId,
            };
            // Check if userId is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                attendance.StatusId = "ERROR";
                attendance.Note = "Không nhận diện được người dùng";
                _attendanceRepo.Add(attendance);
                return null;
            }
            // Check if user is valid for the schedule
            var isUserValidForSchedule = IsUserValidForSchedule(userId, subjectScheduleDetailId);
            if (!isUserValidForSchedule)
            {
                return null;
            }
            // If user is valid, add attendance
            attendance.StatusId = "PRESENT";
            _attendanceRepo.AddAttendanceFromFaceRecognition(attendance, subjectScheduleDetailId);
            var isSuccess = Commit();
            if (!isSuccess)
            {
                _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.faceRecognitionFailed"));
                return null;
            }
            return _userManager.Users.Where(u => u.Id == userId).ProjectTo<UserRes>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

        }

        private string? RecognizeUser(string base64Img)
        {
            try
            {
                // Get the embedding for the input image
                var embedding = _userEmbeddingRepo.GetFaceEmbeddingByBase64Async(base64Img).Result;
                if (embedding == null || embedding.Length == 0)
                    return null;

                // Normalize the input embedding once
                embedding = Normalize(embedding);

                // Retrieve all stored user embeddings
                var users = _userEmbeddingRepo.GetAllEmbeddingsAsync().Result;

                // Initialize variables
                var matchedUserIds = new ConcurrentDictionary<string, double>();
                const double threshold = 0.68;

                // Parallel comparison
                Parallel.ForEach(users, user =>
                {
                    try
                    {
                        var userEmbedding = ParseEmbedding(user.Embedding);
                        if (userEmbedding.Length == 0) return;

                        userEmbedding = Normalize(userEmbedding);

                        // Calculate cosine distance
                        double distance = CosineDistance(embedding, userEmbedding);

                        // Check if the distance is below the threshold
                        if (distance < threshold)
                        {
                            matchedUserIds.TryAdd(user.UserId, distance);
                        }
                    }
                    catch (Exception ex)
                    {
                        _bus.RaiseEvent(new DomainNotification("ERROR", $"{user.UserId}: {ex.Message}"));
                    }
                });

                // Return the best match
                return matchedUserIds.OrderBy(x => x.Value).FirstOrDefault().Key;
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification("ERROR", ex.Message));
                return null;
            }
        }
        private double CosineDistance(double[] v1, double[] v2)
        {
            double dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                dot += v1[i] * v2[i];
                normA += v1[i] * v1[i];
                normB += v2[i] * v2[i];
            }
            return 1 - (dot / (Math.Sqrt(normA) * Math.Sqrt(normB)));
        }

        private double[] ParseEmbedding(string embeddingString)
        {
            if (string.IsNullOrEmpty(embeddingString) || !embeddingString.Contains(','))
                return Array.Empty<double>();
            // Loại bỏ dấu ngoặc vuông nếu có
            embeddingString = embeddingString.Replace("[", "").Replace("]", "");
            return embeddingString.Split(',')
                                  .Select(s => double.Parse(s.Trim(), CultureInfo.InvariantCulture))
                                  .ToArray();
        }
        private double[] Normalize(double[] v)
        {
            double norm = Math.Sqrt(v.Sum(x => x * x));
            return v.Select(x => x / norm).ToArray();
        }

        private class FaceCompareResult
        {
            public bool verified { get; set; }
            public double distance { get; set; }
        }
        private bool IsUserValidForSchedule(string userId, string subjectScheduleDetailId)
        {
            var isStudentInSchedule = _subjectScheduleStudentRepo.GetAll()
            .Where(s => s.Student.UserId == userId)
            .Where(s => s.SubjectSchedule.SubjectScheduleDetails.Any(d => d.SubjectScheduleDetailId == subjectScheduleDetailId)).Any();

            return isStudentInSchedule;
        }
        private FormFile ConvertBase64ToFormFile(string base64Img)
        {
            var imageBytes = Convert.FromBase64String(base64Img.Contains(",") ? base64Img.Split(',')[1] : base64Img);
            return new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "file", "face.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}
