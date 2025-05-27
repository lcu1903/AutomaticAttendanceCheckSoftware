using System.Collections.Generic;
using System.Threading.Tasks;
using AACS.Repository.Implements;
using Core.Interfaces;
using DataAccess.Models;

namespace AACS.Repository.Interface
{
    public interface IUserEmbeddingRepo : IRepository<UserFaceEmbedding>
    {
        Task AddEmbeddingAsync(UserFaceEmbedding embedding);
        Task<List<UserFaceEmbedding>> GetEmbeddingsByUserIdAsync(string userId);
        Task<List<UserFaceEmbedding>> GetAllEmbeddingsAsync();
        Task DeleteEmbeddingsByUserIdAsync(string userId);
        Task<double[]> GetFaceEmbeddingAsync(string imagePath);
        Task<double[]> GetFaceEmbeddingByBase64Async(byte[] imageBytes);
    }
}
