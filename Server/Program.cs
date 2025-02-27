
namespace Controllers;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        if (new[] { "Production" }.Contains(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
        {
            builder = builder.ConfigureAppConfiguration(c =>
            {
                c.AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true);
            });
        }

        return builder
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                // webBuilder.UseKestrel(options =>
                //      {
                //          options.Limits.MaxRequestBodySize = null;
                //      });
            });
            
    }
}