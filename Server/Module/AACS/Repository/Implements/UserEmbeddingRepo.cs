using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using AACS.Repository.Interface;
using DataAccess.Contexts;
using Core.Repository;
using Core.Bus;

namespace AACS.Repository.Implements
{
    public class UserEmbeddingRepo : Repository<UserFaceEmbedding>, IUserEmbeddingRepo
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediatorHandler _bus;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _webPath;
        public UserEmbeddingRepo(ApplicationDbContext dbContext,
            IMediatorHandler bus,
            HttpClient httpClient,
            IConfiguration configuration
         ) : base(dbContext)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _httpClient = httpClient;
            _webPath = configuration.GetValue<string>("JwtIssuerOptions:Issuer") ?? string.Empty;
            _bus = bus;
        }

        public async Task AddEmbeddingAsync(UserFaceEmbedding embedding)
        {
            await _dbContext.UserFaceEmbeddings.AddAsync(embedding);
        }

        public async Task<List<UserFaceEmbedding>> GetEmbeddingsByUserIdAsync(string userId)
        {
            return await _dbContext.UserFaceEmbeddings.Where(e => e.UserId == userId).ToListAsync();
        }

        public async Task<List<UserFaceEmbedding>> GetAllEmbeddingsAsync()
        {
            return await _dbContext.UserFaceEmbeddings.ToListAsync();
        }

        public async Task DeleteEmbeddingsByUserIdAsync(string userId)
        {
            var list = await _dbContext.UserFaceEmbeddings.Where(e => e.UserId == userId).ToListAsync();
            _dbContext.UserFaceEmbeddings.RemoveRange(list);
        }
        public async Task<double[]> GetFaceEmbeddingAsync(string imagePath)
        {

            try
            {
                if (!Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
                {
                    string baseUrl = _webPath.TrimEnd('/');
                    if (!imagePath.StartsWith("/")) imagePath = "/" + imagePath;
                    imagePath = baseUrl + imagePath;
                }
                // Tải ảnh mẫu về file tạm
                var imageBytesUser = _httpClient.GetByteArrayAsync(imagePath).Result;
                var payload = new
                {
                    img = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytesUser),
                    model = "ArcFace"
                };
                var deepFaceApiUrl = _configuration.GetValue<string>("DeepFaceApi:BaseUrl") ?? "http://localhost:5001";
                var response = await _httpClient.PostAsJsonAsync($"{deepFaceApiUrl}/embedding", payload);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<EmbeddingResult>();
                return result?.embedding ?? Array.Empty<double>();
            }
            catch (Exception e)
            {

                return Array.Empty<double>();
            }
        }

        public async Task<double[]> GetFaceEmbeddingByBase64Async(byte[] imageBytes)
        {

            try
            {

                var payload = new
                {
                    img = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes),
                    model = "ArcFace"
                };
                using var client = new HttpClient();
                var deepFaceApiUrl = _configuration.GetValue<string>("DeepFaceApi:BaseUrl") ?? "http://localhost:5001";
                var response = await client.PostAsJsonAsync($"{deepFaceApiUrl}/embedding", payload);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<EmbeddingResult>();
                return result?.embedding ?? Array.Empty<double>();
            }
            catch (Exception e)
            {

                return Array.Empty<double>();
            }
        }
        private class EmbeddingResult
        {
            public double[] embedding { get; set; }
        }
    }
}
