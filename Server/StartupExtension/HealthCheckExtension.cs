using DataAccess.Contexts;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Minio;
using Minio.AspNetCore.HealthChecks;

namespace Controllers.StartupExtensions;

public static class HealthCheckExtension
{
    public static IServiceCollection AddCustomizedHealthCheck(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment env)
    {
        if (env.IsProduction() || env.IsStaging())
        {
            services.AddHealthChecks()
                .AddMinio(sp => (MinioClient)sp.GetRequiredService<IMinioClient>());
            //     .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!)
            //     .AddRedis(configuration.GetValue<string>("RedisSettings:RedisCacheUrl"))
            //     .AddDbContextCheck<ApplicationDbContext>();
            // services.AddHealthChecksUI(opt =>
            // {
            //     opt.SetEvaluationTimeInSeconds(15); // time in seconds between check
            // }).AddInMemoryStorage();
        }

        return services;
    }

    // public static void UseCustomizedHealthCheck(IEndpointRouteBuilder endpoints, IWebHostEnvironment env)
    // {
    //     if (env.IsProduction() || env.IsStaging())
    //     {
    //         endpoints.MapHealthChecks("/hc", new HealthCheckOptions
    //         {
    //             Predicate = _ => true,
    //             ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    //         });

    //         endpoints.MapHealthChecksUI(setup =>
    //         {
    //             setup.UIPath = "/hc-ui";
    //             setup.ApiPath = "/hc-json";
    //         });
    //     }
    // }
}