using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Infrastructure.Repositories;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.Infrastructure.Redis;
using LendTech.Infrastructure.Redis.Interfaces;
using LendTech.Infrastructure.RabbitMQ;
using LendTech.Infrastructure.RabbitMQ.Interfaces;
using LendTech.Infrastructure.RabbitMQ.Options;
namespace LendTech.Infrastructure.Extensions;
/// <summary>
/// Extension متدهای DI Container
/// </summary>
public static class ServiceCollectionExtensions
{
/// <summary>
/// اضافه کردن سرویس‌های Infrastructure
/// </summary>
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
{
// DbContext
services.AddDbContext<LendTechDbContext>(options =>
{
options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});
    // Repositories
    services.AddRepositories();

    // Redis
    services.AddRedisCache(configuration);

    // RabbitMQ
    services.AddRabbitMQ(configuration);

    return services;
}

/// <summary>
/// اضافه کردن Repository ها
/// </summary>
private static IServiceCollection AddRepositories(this IServiceCollection services)
{
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IPermissionRepository, PermissionRepository>();
    services.AddScoped<IPermissionGroupRepository, PermissionGroupRepository>();
    services.AddScoped<IOrganizationRepository, OrganizationRepository>();
    services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    services.AddScoped<ICurrencyRepository, CurrencyRepository>();
    services.AddScoped<IOutboxEventRepository, OutboxEventRepository>();
    services.AddScoped<IInboxEventRepository, InboxEventRepository>();

    return services;
}

/// <summary>
/// اضافه کردن Redis
/// </summary>
private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
{
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = configuration.GetConnectionString("Redis");
        options.InstanceName = "LendTech:";
    });

    services.AddSingleton<ICacheService, CacheService>();

    return services;
}

/// <summary>
/// اضافه کردن RabbitMQ
/// </summary>
private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
    
    services.AddSingleton<IMessagePublisher, MessagePublisher>();
    services.AddSingleton<IMessageConsumer, MessageConsumer>();

    return services;
}
}
