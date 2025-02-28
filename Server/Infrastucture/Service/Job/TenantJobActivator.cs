// using Hangfire;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.DependencyInjection;

// public class TenantJobActivator : JobActivator
// {
//     private readonly IServiceProvider _serviceProvider;
//     private readonly IHttpContextAccessor _httpContextAccessor;

//     public TenantJobActivator(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
//     {
//         _serviceProvider = serviceProvider;
//         _httpContextAccessor = httpContextAccessor;
//     }

//     public override object ActivateJob(Type type)
//     {
//         var tenantId = _httpContextAccessor.HttpContext?.Items["TenantId"]?.ToString();
//         var scope = _serviceProvider.CreateScope();
//         var service = scope.ServiceProvider.GetRequiredService(type);

//         if (service is ITenantAware tenantAwareService)
//         {
//             tenantAwareService.SetTenantId(tenantId); 
//         }

//         return service;
//     }
// }