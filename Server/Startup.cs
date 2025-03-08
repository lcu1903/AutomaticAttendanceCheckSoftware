using System.Threading.RateLimiting;
using Controllers.StartupExtensions;
using Microsoft.AspNetCore.Http.Features;
using MediatR;
using Microsoft.AspNetCore.SpaServices.Extensions;
using CrosCuttingIoC;


namespace Controllers;

public class Startup
{
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
        services.AddDistributedMemoryCache();

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
        // services.AddHangfire(Configuration);

        // ----- In memory cache -----
        services.AddMemoryCache();

        // // ----- Refit -----
        // services.AddRefitExtension(Configuration, _env);

        // // ----- Logging -----
        // services.AddLoggingExtension(Configuration, _env);
        // services.AddOpenTelemetryConfig(Configuration, _env);

        // // ---- Redis cache ----
        // services.AddCaching(Configuration);
        // services.AddResponseCaching();
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
    public void Configure(IApplicationBuilder app
    // ApplicationDbSeeder seeder,

    )
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
            endpoints.MapControllers();
            // .RequireRateLimiting("token");
            // // endpoints.MapRazorPages();
            // // ----- Health check -----
            // HealthCheckExtension.UseCustomizedHealthCheck(endpoints, _env);
        });

        // ----- Swagger UI -----
        app.UseCustomizedSwagger(_env);

        // ----- Hangfire -----
        // app.UseHangfireDashboard(Configuration);


        // Each map its own static files otherwise
        // it will only ever serve index.html no matter the filename 
        // ----- SPA -----


        app.Map("", admin =>
        {
            admin.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Client";
                if (_env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        });
        // ApplicationDataInitializer.Initialize(app.ApplicationServices);
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Adding dependencies from another layers (isolated from Presentation)
        NativeInjector.RegisterServices(services);
    }
}