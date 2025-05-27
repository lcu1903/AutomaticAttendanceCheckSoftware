using AACS.Service.Interface;
using OpenCvSharp;
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
using Minio;
using AACS.Repository.Interface;
using System.Linq;
using System.Globalization;

namespace AACS.Service.Implements
{
    public class FaceDetectionService : DomainService, IFaceDetectionService
    {
        private readonly string _cascadePath;
        private readonly string _webPath;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUserEmbeddingRepo _userEmbeddingRepo;

        public FaceDetectionService(
             UserManager<ApplicationUser> userManager,
            INotificationHandler<DomainNotification> notifications,
            IConfiguration configuration,
            HttpClient httpClient,
            IUserEmbeddingRepo userEmbeddingRepo,
            IMapper mapper,
            IUnitOfWork uow,
            IMediatorHandler bus
        ) : base(notifications, uow, bus)
        {
            _cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cascades", "haarcascade_frontalface_default.xml");
            _configuration = configuration;
            _userEmbeddingRepo = userEmbeddingRepo;
            _httpClient = httpClient;
            _webPath = configuration.GetValue<string>("JwtIssuerOptions:Issuer") ?? string.Empty;
            _userManager = userManager;
            _mapper = mapper;
        }


        public UserRes? FaceRecognitionAsync(byte[] imageData, string subjectScheduleId)
        {
            var userId = RecognizeUser(imageData);
            if (string.IsNullOrEmpty(userId))
                return null;
            return _userManager.Users.Where(u => u.Id == userId).ProjectTo<UserRes>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

        }

        public string? RecognizeUser(byte[] imageBytes)
        {
            var embedding = _userEmbeddingRepo.GetFaceEmbeddingByBase64Async(imageBytes).Result;
            if (embedding == null || embedding.Length == 0)
                return null;
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
                    Console.WriteLine($"Error processing user {user.UserId}: {ex.Message}");
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
