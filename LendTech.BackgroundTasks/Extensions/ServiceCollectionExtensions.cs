using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LendTech.BackgroundTasks.Services;
using LendTech.BackgroundTasks.Options;
namespace LendTech.BackgroundTasks.Extensions;
/// <summary>
/// Extension متدهای DI Container
/// </summary>
public static class ServiceCollectionExtensions
{
/// <summary>
/// اضافه کردن Background Services
/// </summary>
public static IServiceCollection AddBackgroundTaskServices(this IServiceCollection services, IConfiguration configuration)
{
// تنظیمات
services.Configure<OutboxOptions>(configuration.GetSection("BackgroundTasks:Outbox"));
services.Configure<InboxOptions>(configuration.GetSection("BackgroundTasks:Inbox"));
services.Configure<CacheRefreshOptions>(configuration.GetSection("BackgroundTasks:CacheRefresh"));
services.Configure<CleanupOptions>(configuration.GetSection("BackgroundTasks:Cleanup"));
    // Hosted Services
    services.AddHostedService<OutboxDispatcherService>();
    services.AddHostedService<InboxProcessorService>();
    services.AddHostedService<CacheRefreshService>();
    services.AddHostedService<CleanupService>();

    return services;
}

/// <summary>
/// اضافه کردن Background Services به صورت انتخابی
/// </summary>
public static IServiceCollection AddBackgroundTaskServices(
    this IServiceCollection services, 
    IConfiguration configuration,
    bool enableOutbox = true,
    bool enableInbox = true,
    bool enableCacheRefresh = true,
    bool enableCleanup = true)
{
    // تنظیمات
    if (enableOutbox)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("BackgroundTasks:Outbox"));
        services.AddHostedService<OutboxDispatcherService>();
    }

    if (enableInbox)
    {
        services.Configure<InboxOptions>(configuration.GetSection("BackgroundTasks:Inbox"));
        services.AddHostedService<InboxProcessorService>();
    }

    if (enableCacheRefresh)
    {
        services.Configure<CacheRefreshOptions>(configuration.GetSection("BackgroundTasks:CacheRefresh"));
        services.AddHostedService<CacheRefreshService>();
    }

    if (enableCleanup)
    {
        services.Configure<CleanupOptions>(configuration.GetSection("BackgroundTasks:Cleanup"));
        services.AddHostedService<CleanupService>();
    }

    return services;
}
}
