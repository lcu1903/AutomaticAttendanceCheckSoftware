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
        private readonly IDistributedCache _cache;
        private const string EmbeddingCacheKey = "UserEmbeddings";
        public FaceDetectionService(
             UserManager<ApplicationUser> userManager,
            IStorageService storageService,
            INotificationHandler<DomainNotification> notifications,
            IAttendanceRepo attendanceRepo,
            IUserEmbeddingRepo userEmbeddingRepo,
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
            _bus = bus;
            _cache = cache;
        }


        public UserRes? FaceRecognitionAsync(string base64Img, string subjectScheduleDetailId)
        {
            var imageBytes = Convert.FromBase64String(base64Img.Contains(",") ? base64Img.Split(',')[1] : base64Img);
            var imageFile = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "file", "face.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
            var imageUrl = _storageService.UploadObjectAsync(imageFile, default).Result;

            var userId = RecognizeUser(base64Img);
            var attendance = new Attendance
            {
                UserId = userId,
                AttendanceId = Guid.NewGuid().ToString(),
                AttendanceTime = DateTime.UtcNow,
                ImageUrl = imageUrl,
                SubjectScheduleDetailId = subjectScheduleDetailId,
            };
            if (string.IsNullOrEmpty(userId))
            {
                attendance.StatusId = "ERROR";
                attendance.Note = "Không nhận diện được người dùng";
                _attendanceRepo.Add(attendance);
                return null;
            }

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
            // // Try to get data from cache
            // var cachedData = _cache.GetString(EmbeddingCacheKey);
            // var cachedEmbedding = new List<UserFaceEmbedding>();
            // if (!string.IsNullOrEmpty(cachedData))
            // {
            //     cachedEmbedding.AddRange(JsonSerializer.Deserialize<List<UserFaceEmbedding>>(cachedData));
            // }
            var embedding = _userEmbeddingRepo.GetFaceEmbeddingByBase64Async(base64Img).Result;
            if (embedding == null || embedding.Length == 0)
                return null;
            // var userImageVectors = cachedEmbedding.Count > 0 ? cachedEmbedding : _userEmbeddingRepo.GetAllEmbeddingsAsync().Result;
            var userImageVectors = _userEmbeddingRepo.GetAllEmbeddingsAsync().Result;
            string? matchedUserId = null;
            double minDistance = double.MaxValue;
            const double threshold = 0.68;
            var users = _userEmbeddingRepo.GetAllEmbeddingsAsync().Result;
            Parallel.ForEach(users, (user, state) =>
            {
                try
                {
                    var userEmbedding = ParseEmbedding(user.Embedding);
                    if (userEmbedding.Length == 0) return;
                    userEmbedding = Normalize(userEmbedding);
                    embedding = Normalize(embedding);

                    // Tính khoảng cách cosine
                    double distance = CosineDistance(embedding, userEmbedding);

                    // So sánh với ngưỡng
                    if (distance < threshold && distance < minDistance)
                    {
                        minDistance = distance;
                        matchedUserId = user.UserId;
                        state.Break();
                    }
                }
                catch (Exception ex)
                {
                    _bus.RaiseEvent(new DomainNotification("ERROR", $"{user.UserId}: {ex.Message}"));
                    throw ex;
                }
            });
            return matchedUserId;
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
        public double distance { get; set; }
    }
}
