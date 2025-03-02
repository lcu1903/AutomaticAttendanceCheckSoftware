
using Core.Bus;
using Core.Interfaces;
using Core.Notifications;
using DataAccess.Models;
using Infrastructure.Authorization;
using Infrastructure.Services.Job;
using Infrastructure.UoW;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
namespace CrosCuttingIoC;

public static class NativeInjector
{
    public static void RegisterServices(IServiceCollection services)
    {
        // ASP.NET HttpContext dependency
        services.AddHttpContextAccessor();
        // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Systems.Domain Bus (Mediator)
        services.AddScoped<IMediatorHandler, InMemoryBus>();

        // Exception Middleware

        // ASP.NET Authorization Polices
        services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();

        // Minio Service
        // services.AddScoped<IStorageService,StorageService>();
        // services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IJobService, HangfireService>();
        //Configuration - General Setting Service

        // Systems.Domain - Events
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

        // Systems.Domain - 3rd parties
        // services.AddScoped<IHttpService, HttpService>();
        // services.AddTransient<IMailService, SmtpMailService>();
        // services.AddTransient<IEmailTemplateService, EmailTemplateService>();


        // Master.Application
        // System - Data Access


        services.AddScoped<IUnitOfWork, UnitOfWork>();


        // services.AddScoped<ISMSServices, SMSServices>();
        //Attendance Background Service
        // services.AddHostedService<SystemBackgroundService>();
        // Infra - Identity Services
        // services.AddTransient<IEmailSender, AuthEmailMessageSender>();
        // services.AddTransient<ISmsSender, AuthSmsMessageSender>();

        // Infra - Identity
        services.AddScoped<IUser, AspNetUser>();
        // services.AddSingleton<IJwtFactory, JwtFactory>();

        //Infra - Seeding
        // services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        // services.AddTransient<ApplicationDbInitializer>();
        // services.AddTransient<ApplicationDbSeeder>();
        services.AddServices(typeof(IScopedService), ServiceLifetime.Scoped);

        // //Annoucement
        // services.AddScoped<IAnnouncementService, AnnounceService>();
        // services.AddScoped<IAnnouncementRepo, AnnoucementRepo>();
        // //Weblink helper
        // services.AddScoped<IWebLinkHelper, WebLinkHelper>();
        // services.AddScoped<IConfigurationRepo, ConfigurationRepo>();

    }

    private static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime) =>
        lifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
        };

    private static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
    {
        var interfaceTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => interfaceType.IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false })
                .Select(t => new
                {
                    Service = t.GetInterfaces().FirstOrDefault(d => d.Name == $"I{t.Name}"),
                    Implementation = t
                }
                )
                .Where(t => t.Service is not null
                            && interfaceType.IsAssignableFrom(t.Service));

        foreach (var type in interfaceTypes)
        {
            services.AddService(type.Service!, type.Implementation, lifetime);
        }

        return services;
    }
}