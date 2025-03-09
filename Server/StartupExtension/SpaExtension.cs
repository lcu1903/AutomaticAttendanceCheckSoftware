

namespace StartupExtensions;

public static class SpaExtension
{
    public static void AddSpaStaticFiles(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSpaStaticFiles((options) =>
        {
            options.RootPath = "ClientApp/dist";
        });
    }
}