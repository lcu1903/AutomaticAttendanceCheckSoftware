using System.Models;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Contexts;

public partial class ApplicationDbContext
{
   public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public virtual DbSet<ApplicationUser> Users { get; set; }

    public static void SystemOnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token);
        });
    }

    
}