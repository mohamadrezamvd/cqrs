using Microsoft.Extensions.DependencyInjection;
using LendTech.Application.Extensions;
using LendTech.Infrastructure.Extensions;
using LendTech.BackgroundTasks.Extensions;
namespace LendTech.API.Extensions;
/// <summary>
/// Extension method های ثبت سرویس‌ها
/// </summary>
public static class ServiceRegistrationExtensions
{
/// <summary>
/// ثبت تمام سرویس‌های برنامه
/// </summary>
public static IServiceCollection AddLendTechServices(
this IServiceCollection services,
IConfiguration configuration)
{
// Infrastructure Services
services.AddInfrastructureServices(configuration);
    // Application Services  
    services.AddApplicationServices();
    services.AddValidationRules();

    // Background Services
    services.AddBackgroundTaskServices(configuration,
        enableOutbox: true,
        enableInbox: true,
        enableCacheRefresh: true,
        enableCleanup: true);

    // API Services
    services.AddApiServices(configuration);

    return services;
}

/// <summary>
/// ثبت سرویس‌های API
/// </summary>
private static IServiceCollection AddApiServices(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // Controllers
    services.AddControllers(options =>
    {
        options.ModelValidatorProviders.Clear();
        options.SuppressAsyncSuffixInActionNames = false;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    // API Explorer
    services.AddEndpointsApiExplorer();

    // Swagger
    services.AddSwaggerConfiguration();

    // Authentication & Authorization
    services.AddJwtAuthentication(configuration);
    services.AddPermissionBasedAuthorization();

    // CORS
    services.AddCorsConfiguration(configuration);

    // Health Checks
    services.AddHealthChecksConfiguration(configuration);

    // Rate Limiting
    services.AddRateLimiting(configuration);

    // API Versioning
    services.AddApiVersioningConfiguration();

    // HttpContext Accessor
    services.AddHttpContextAccessor();

    // Response Caching
    services.AddResponseCaching();

    // Response Compression
    services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });

    return services;
}
}
