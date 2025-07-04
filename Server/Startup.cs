using System.Threading.RateLimiting;
using StartupExtensions;
using Microsoft.AspNetCore.Http.Features;
using MediatR;
using Microsoft.AspNetCore.SpaServices.Extensions;
using CrosCuttingIoC;
using Hangfire;
using System.Storage;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;


namespace Controllers;

public class Startup
{
    private Process? _pythonProcess;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment _env;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // ExcelPackage.LicenseContext = LicenseContext.Commercial; // or LicenseContext.NonCommercial depending on your license type

        // ----- Rate Limit -----
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.AddPolicy("token", httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    httpContext.Request.Headers["X-Forwarded-For"].ToString(),
                    factory: _ => new TokenBucketRateLimiterOptions()
                    {
                        TokenLimit = 500,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                        TokensPerPeriod = 50,
                        AutoReplenishment = true,
                    }));
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        // services.AddRateLimitService(Configuration); 

        // ----- Database -----
        services.AddCustomizedDatabase(Configuration, _env);

        // ----- Auth -----
        services.AddCustomizedAuth(Configuration);


        // ----- AutoMapper -----
        services.AddAutoMapperSetup();

        // Add session SErvices
        services.AddSession();
        // Add data protection services
        services.AddDataProtection();
        // Add distributed cache 
        // services.AddDistributedMemoryCache();

        // Adding MediatR for Systems.Domain Events and Notifications
        services.AddMediatR(d => d.RegisterServicesFromAssemblyContaining(typeof(Startup)));

        services.AddCustomizedHash(Configuration);


        // // ----- Swagger UI -----
        services.AddCustomizedSwagger(_env);

        // ----- Health check -----
        services.AddCustomizedHealthCheck(Configuration, _env);

        // ----- Minio -----
        services.AddCustomizedMinio(Configuration);


        // ----- Hangfire -----
        services.AddHangfire(Configuration);


        // ----- In memory cache -----
        services.AddRedisCache(Configuration);
        // // ----- Logging -----
        // services.AddLoggingExtension(Configuration, _env);
        // services.AddOpenTelemetryConfig(Configuration, _env);

        services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
        });

        // ----- Kubernetes client ----
        // services.AddSingleton<IKubernetes>(_ =>
        //     new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile("./KubernetesConfig/config")));

        // .NET Native DI Abstraction
        RegisterServices(services);


        services.AddControllers();
        // .AddNewtonsoftJson(x =>
        // {
        //     // x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        //     x.SerializerSettings.ContractResolver = new DefaultContractResolver
        //     {
        //         NamingStrategy = new CamelCaseNamingStrategy()
        //     };
        //     x.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";

        //     x.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        //     // x.SerializerSettings.DateFormatString = DateFormatString.IsoDateFormat ;
        //     x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        // });

        // ----- SPA -----
        services.AddSpaStaticFiles(Configuration);

    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
        // context.Database.Migrate();
        // seeder.SeedDataAndVerifyDataAsync(new CancellationToken()).GetAwaiter().GetResult();
        // samlService.RebuildAllExistingSsoProvidersAsync(true).GetAwaiter().GetResult();


        // ----- Error Handling -----
        app.UseCustomizedErrorHandling(_env);

        app.UseRouting();

        app.UseRateLimiter();

        app.UseResponseCaching();


        app.UseStaticFiles();


        if (!_env.IsDevelopment())
        {
            app.UseSpaStaticFiles();
        }

        // ----- CORS -----
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        // ----- Auth -----
        app.UseCustomizedAuth();



        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers()
            .RequireRateLimiting("token");
            // // ----- Health check -----
            // HealthCheckExtension.UseCustomizedHealthCheck(endpoints, _env);
        });

        // ----- Swagger UI -----
        app.UseCustomizedSwagger(_env);

        // ----- Hangfire -----
        app.UseHangfireDashboard(Configuration);
        RecurringJob.AddOrUpdate<IStorageService>(
         "CleanupUnusedObjects",
         service => service.CleanupUnusedObjectsAsync(CancellationToken.None),
         () => Cron.Daily(),
         new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }); // hoặc Cron theo thời gian bạn mong muốn


        //Redis
        // app.UseRedisCache(Configuration);

        // Each map its own static files otherwise
        // it will only ever serve index.html no matter the filename 
        // ----- SPA -----


        app.Map("", admin =>
        {
            admin.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";
                if (_env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
                if (env.IsProduction())
                {
                    spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                    };
                }
            });
        });
        // ApplicationDataInitializer.Initialize(app.ApplicationServices);

        // Khởi động DeepFace API server nếu chưa chạy
        // try
        // {
        //     var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Module/AACS/FaceRecognition/face_compare.py");
        //     Console.WriteLine($"Script exists: {File.Exists(scriptPath)} - Path: {scriptPath}");
        //     _pythonProcess = new Process
        //     {
        //         StartInfo = new ProcessStartInfo
        //         {
        //             FileName = "python",
        //             Arguments = "Module/AACS/FaceRecognition/face_compare.py",
        //             WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
        //             UseShellExecute = false,
        //             CreateNoWindow = true
        //         }
        //     };
        //     _pythonProcess.Start();
        //     lifetime.ApplicationStopping.Register(() =>
        //     {
        //         try
        //         {
        //             if (_pythonProcess != null && !_pythonProcess.HasExited)
        //             {
        //                 _pythonProcess.Kill(true);
        //                 _pythonProcess.Dispose();
        //             }
        //         }
        //         catch { /* log nếu cần */ }
        //     });
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine("BaseDirectory: " + AppDomain.CurrentDomain.BaseDirectory);
        //     Console.WriteLine("Python script full path: " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Module/AACS/FaceRecognition/face_compare.py"));
        //     Console.WriteLine("Error starting Python process: " + ex.Message);
        //     throw;
        // }
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Adding dependencies from another layers (isolated from Presentation)
        NativeInjector.RegisterServices(services);
    }
}