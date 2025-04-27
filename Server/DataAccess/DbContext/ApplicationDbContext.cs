using System.Security.Claims;
using DataAccess.Models;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Contexts;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration)
    : IdentityDbContext<ApplicationUser>(options)
{
    private readonly IConfiguration _configuration = configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {


        SystemOnModelCreating(builder);
        // CmmsOnModelCreating(builder);
        // MasterOnModelCreating(builder);
        // NotificationOnModelCreating(builder);
        base.OnModelCreating(builder);

    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaving()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is EntityAudit)
            .ToList();
        // UpdateSoftDelete(entities);
        UpdateTimestamps(entities);
    }


    private void UpdateTimestamps(List<EntityEntry> entries)
    {
        var filtered = entries
            .Where(x => x.State is EntityState.Added or EntityState.Modified);

        // TODO: Get real current user id
        Console.WriteLine("CONTEXT", httpContextAccessor?.HttpContext?.User);
        var currentUserId = httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "guest";

        foreach (var entry in filtered)
        {

            if (entry.State == EntityState.Added)
            {
                ((EntityAudit)entry.Entity).CreateDate = DateTime.UtcNow;
                ((EntityAudit)entry.Entity).CreatedUserId = currentUserId;
            }

            ((EntityAudit)entry.Entity).UpdateDate = DateTime.UtcNow;
            ((EntityAudit)entry.Entity).UpdatedUserId = currentUserId;

        }

    }
}