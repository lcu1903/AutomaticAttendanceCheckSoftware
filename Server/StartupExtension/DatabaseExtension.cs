
using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Controllers.StartupExtensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddCustomizedDatabase(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment env)
    {
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            // if (!env.IsProduction())
            // {
            options.UseSeeding( (context, _) =>
            {
                var superUser =  context.Set<ApplicationUser>().FirstOrDefault(b => b.Id == "Admin");
                if (superUser == null)
                {
                    context.Set<ApplicationUser>().Add(new ApplicationUser { 
                        Id = "Admin",
                        UserName = "Admin", 
                        Email = "", 
                        EmailConfirmed = true, 
                        PhoneNumber = "0123456789",
                        FullName = "Administrator",
                        PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "123456"), 
                        // PhoneNumberConfirmed = true, 
                        // TwoFactorEnabled = false, 
                        // LockoutEnabled = false, 
                        // AccessFailedCount = 0, 
                        // SecurityStamp = Guid.NewGuid().ToString() 
                        });
                    
                    context.Set<IdentityRole>().Add(new IdentityRole {  Id="Admin", Name = "Admin", NormalizedName = "ADMIN" });
                    context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string> { RoleId = "Admin", UserId = "Admin" });
                    context.SaveChanges();
                }
            });
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
            // }
        });
           


        return services;
    }
}