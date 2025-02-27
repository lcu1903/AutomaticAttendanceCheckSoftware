using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Controllers.StartupExtensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services, IWebHostEnvironment env)
    {
        // if (env.IsDevelopment() || env.IsStaging())
        // {

            services.AddSwaggerGen(setup =>
            {
                setup.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
                setup.SupportNonNullableReferenceTypes();
                setup.UseAllOfToExtendReferenceSchemas();
                setup.UseAllOfForInheritance();
                setup.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                var xmlFiles = new[]
                {
                    //the current assembly (the web api)
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",

                    //another assembly housing MyEntityView. Add more lines like this to add more assemblies
                    // $"{Assembly.GetAssembly(typeof(MasterDataProduct))?.GetName().Name}.xml"
                };

                foreach (var xmlFile in xmlFiles)
                {
                    var xmlCommentFile = xmlFile;
                    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                    if (File.Exists(xmlCommentsFullPath))
                    {
                        setup.IncludeXmlComments(xmlCommentsFullPath);
                    }
                }

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });

            });
        // }

        return services;
    }

    public static IApplicationBuilder UseCustomizedSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsStaging())
        {
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(o => { o.SerializeAsV2 = true; });
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API document");
            });
            // }


        }
        return app;
    }

}
public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Add to model.Required all properties where Nullable is false.
    /// </summary>
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        var additionalRequiredProps = model.Properties
            .Where(x => !x.Value.Nullable && !model.Required.Contains(x.Key))
            .Select(x => x.Key);
        foreach (var propKey in additionalRequiredProps)
        {
            model.Required.Add(propKey);
        }
    }
}