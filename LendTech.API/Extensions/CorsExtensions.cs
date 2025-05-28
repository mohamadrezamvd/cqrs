namespace LendTech.API.Extensions;
/// <summary>
/// Extension methods برای CORS
/// </summary>
public static class CorsExtensions
{
/// <summary>
/// اضافه کردن تنظیمات CORS
/// </summary>
public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
{
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    services.AddCors(options =>
    {
        options.AddPolicy("LendTechPolicy", builder =>
        {
            if (allowedOrigins.Contains("*"))
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }
            else
            {
                builder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }
        });
    });

    return services;
}
}
