using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace LendTech.API.Extensions;
/// <summary>
/// Extension methods برای Swagger
/// </summary>
public static class SwaggerExtensions
{
/// <summary>
/// اضافه کردن تنظیمات Swagger
/// </summary>
public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
{
services.AddSwaggerGen(options =>
{
options.SwaggerDoc("v1", new OpenApiInfo
{
Title = "LendTech API",
Version = "v1",
Description = "سیستم مدیریت تسهیلات LendTech",
Contact = new OpenApiContact
{
Name = "تیم توسعه LendTech",
Email = "dev@lendtech.com"
},
License = new OpenApiLicense
{
Name = "MIT",
Url = new Uri("https://opensource.org/licenses/MIT")
}
});
        // JWT Authentication
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // XML Comments
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // Custom Operation Filter
        options.OperationFilter<SwaggerDefaultValues>();
    });

    return services;
}
}
/// <summary>
/// Operation Filter برای مقادیر پیش‌فرض
/// </summary>
public class SwaggerDefaultValues : IOperationFilter
{
public void Apply(OpenApiOperation operation, OperationFilterContext context)
{
var apiDescription = context.ApiDescription;
    operation.Deprecated |= apiDescription.IsDeprecated();

    if (operation.Parameters == null)
        return;

    foreach (var parameter in operation.Parameters)
    {
        var description = apiDescription.ParameterDescriptions
            .First(p => p.Name == parameter.Name);

        parameter.Description ??= description.ModelMetadata?.Description;

        if (parameter.Schema.Default == null && 
            description.DefaultValue != null)
        {
            parameter.Schema.Default = new Microsoft.OpenApi.Any.OpenApiString(
                description.DefaultValue.ToString());
        }

        parameter.Required |= description.IsRequired;
    }
}
}
