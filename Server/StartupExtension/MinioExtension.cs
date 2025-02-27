using Minio;
using Minio.AspNetCore;
namespace Controllers.StartupExtensions;

public static class MinioExtension
{
    public static void AddCustomizedMinio(this IServiceCollection services, IConfiguration configuration)
    {
        var endpoint = configuration.GetValue<string>("Minio:Endpoint");
        // var clientEndpoint = configuration.GetValue<string>("Minio:ClientEndpoint");
        var accessKey = configuration.GetValue<string>("Minio:AccessKey");
        var secretKey = configuration.GetValue<string>("Minio:SecretKey");
        services.AddMinio(options =>
        {
            options.Endpoint = endpoint;
            options.AccessKey = accessKey;
            options.SecretKey = secretKey;
            options.ConfigureClient(client =>
            {
                client.WithEndpoint(options.Endpoint).WithCredentials(accessKey, secretKey).Build();
            });

        });
        // services.AddTransient<AuthHeaderHandler>();
        // services.AddRefitClient<IMinioApi>().ConfigureHttpClient(c =>
        // {
        //     c.BaseAddress = new Uri(clientEndpoint);
        // }).AddHttpMessageHandler(()=> new AuthHeaderHandler(configuration,RestService.For<IMinioAuthApi>(clientEndpoint)));

        // services.AddRefitClient<IMinioAuthApi>().ConfigureHttpClient(c =>
        // {
        //     c.BaseAddress = new Uri(clientEndpoint);
        // }).AddHttpMessageHandler(()=> new AuthHeaderHandler(configuration,RestService.For<IMinioAuthApi>(clientEndpoint)));
    }
    // class AuthHeaderHandler : DelegatingHandler
    // {
    //     private readonly IConfiguration _configuration;
    //     private readonly IMinioAuthApi _minioAuthApi;

    //     public AuthHeaderHandler(IConfiguration configuration, IMinioAuthApi minioAuthApi)
    //     {
    //         _configuration = configuration;
    //         _minioAuthApi = minioAuthApi;

    //     }

    //     protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
    //         CancellationToken cancellationToken)
    //     {
    //             var tokenRequest = await Login();
    //             string token = null;
    //             if (tokenRequest.Headers.TryGetValues("Set-Cookie", out var cookiesReq))
    //             {

    //                 foreach (var cookie in cookiesReq)
    //                 {
    //                     if (cookie.StartsWith("token"))
    //                     {
    //                         var cookieValue = cookie.Split(';').FirstOrDefault(c => c.StartsWith("token"));
    //                         if (cookieValue != null)
    //                         {
    //                             token = cookieValue.Substring(cookieValue.IndexOf('=') + 1);
    //                             break;
    //                         }
    //                     }
    //                 }
    //             }



    //         request.Headers.TryAddWithoutValidation("Cookie", $"token={token}");


    //         var response = await base.SendAsync(request, cancellationToken);
    //         if (response.StatusCode == HttpStatusCode.Unauthorized)
    //         {
    //             var requestToken = await Login();
    //             request.Headers.Remove("Cookie");
    //             if (requestToken.Headers.TryGetValues("Set-Cookie", out var cookiesRes))
    //             {

    //                 foreach (var cookie in cookiesRes)
    //                 {
    //                     if (cookie.StartsWith("token"))
    //                     {
    //                         var cookieValue = cookie.Split(';').FirstOrDefault(c => c.StartsWith("token"));
    //                         if (cookieValue != null)
    //                         {
    //                             token = cookieValue.Substring(cookieValue.IndexOf('=') + 1);
    //                             break;
    //                         }
    //                     }
    //                 }
    //             }
    //             request.Headers.TryAddWithoutValidation("Set-Cookie", $"token={token}");
    //             response = await base.SendAsync(request, cancellationToken);
    //         }

    //         return response;
    //     }

    //     private async Task<HttpResponseMessage> Login()
    //     {
    //         var accessKey = _configuration.GetValue<string>("Minio:Username");
    //         var secretKey = _configuration.GetValue<string>("Minio:Password");

    //         var token = await _minioAuthApi.Login(new MinioLoginRequest
    //         {
    //             AccessKey = accessKey,
    //             SecretKey = secretKey
    //         });
    //         return token;
    //     }
    // }
}