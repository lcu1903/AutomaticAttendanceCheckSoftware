using System.Models;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Contexts;

public partial class ApplicationDbContext
{
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public virtual DbSet<ApplicationUser> Users { get; set; }
    public virtual DbSet<SystemPage> SystemPages { get; set; } = null!;

    public static void SystemOnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token);
        });
        modelBuilder.Entity<SystemPage>(entity =>
        {
            entity.HasKey(e => e.PageId);
            entity.Property(e => e.PageId).HasMaxLength(128);
            entity.Property(e => e.PageName).HasMaxLength(128);
            entity.Property(e => e.PageUrl).HasMaxLength(128);
            entity.Property(e => e.PageIcon).HasMaxLength(128);
            entity.Property(e => e.ParentId).HasMaxLength(128);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
        });
    }
}