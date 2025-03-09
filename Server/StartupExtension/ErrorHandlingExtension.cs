namespace StartupExtensions;

public static class ErrorHandlingExtension
{
    public static IApplicationBuilder UseCustomizedErrorHandling(this IApplicationBuilder app, IWebHostEnvironment env)
    {
             
        // Register exception middleware error
            
        if (!env.IsProduction())
        {
            app.UseDeveloperExceptionPage();
        }
            

        return app;
    }
}