using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Models;

namespace DataAccess.Models
{
    public partial class UserFaceEmbedding
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Embedding { get; set; } = null!; // Lưu JSON hoặc base64
    }
}
