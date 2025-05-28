using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.BackgroundTasks.Options;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LendTech.BackgroundTasks.Services;
/// <summary>
/// سرویس به‌روزرسانی کش
/// </summary>
public class CacheRefreshService : BackgroundService
{
private readonly IServiceProvider _serviceProvider;
private readonly ILogger<CacheRefreshService> _logger;
private readonly IDistributedCache _cache;
private readonly CacheRefreshOptions _options;
public CacheRefreshService(
    IServiceProvider serviceProvider,
    ILogger<CacheRefreshService> logger,
    IDistributedCache cache,
    IOptions<CacheRefreshOptions> options)
{
    _serviceProvider = serviceProvider;
    _logger = logger;
    _cache = cache;
    _options = options.Value;
}

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("سرویس Cache Refresh شروع به کار کرد");

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            await RefreshCaches(stoppingToken);
            
            // تاخیر بین به‌روزرسانی‌ها
            await Task.Delay(TimeSpan.FromMinutes(_options.RefreshIntervalMinutes), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در به‌روزرسانی کش");
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    _logger.LogInformation("سرویس Cache Refresh متوقف شد");
}

/// <summary>
/// به‌روزرسانی کش‌ها
/// </summary>
private async Task RefreshCaches(CancellationToken cancellationToken)
{
    _logger.LogDebug("شروع به‌روزرسانی کش‌ها");

    using var scope = _serviceProvider.CreateScope();

    // به‌روزرسانی کش سازمان‌ها
    if (_options.RefreshOrganizationCache)
    {
        await RefreshOrganizationCaches(scope.ServiceProvider, cancellationToken);
    }

    // به‌روزرسانی کش نرخ ارز
    if (_options.RefreshCurrencyRateCache)
    {
        await RefreshCurrencyRateCaches(scope.ServiceProvider, cancellationToken);
    }

    // به‌روزرسانی کش دسترسی‌های کاربران فعال
    if (_options.RefreshActiveUserPermissionsCache)
    {
        await RefreshActiveUserPermissionsCaches(scope.ServiceProvider, cancellationToken);
    }

    _logger.LogDebug("به‌روزرسانی کش‌ها کامل شد");
}

/// <summary>
/// به‌روزرسانی کش سازمان‌ها
/// </summary>
private async Task RefreshOrganizationCaches(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    try
    {
        var organizationRepository = serviceProvider.GetRequiredService<IOrganizationRepository>();
        var organizations = await organizationRepository.GetAllAsync(cancellationToken);

        foreach (var org in organizations.Where(o => o.IsActive))
        {
            var cacheKey = CacheKeys.Organization(org.Id.ToString());
            var cacheValue = org.ToJson();
            
            await _cache.SetStringAsync(
                cacheKey, 
                cacheValue,
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(CacheKeys.LongCacheMinutes)
                },
                cancellationToken);

            // کش Feature Flags
            var features = await organizationRepository.GetFeaturesAsync(org.Id, cancellationToken);
            var featureCacheKey = CacheKeys.OrganizationFeatures(org.Id.ToString());
            
            await _cache.SetStringAsync(
                featureCacheKey,
                features.ToJson(),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(CacheKeys.LongCacheMinutes)
                },
                cancellationToken);
        }

        _logger.LogInformation("کش {Count} سازمان به‌روزرسانی شد", organizations.Count);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در به‌روزرسانی کش سازمان‌ها");
    }
}

/// <summary>
/// به‌روزرسانی کش نرخ ارز
/// </summary>
private async Task RefreshCurrencyRateCaches(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    try
    {
        var currencyRepository = serviceProvider.GetRequiredService<ICurrencyRepository>();
        var currencies = await currencyRepository.GetActiveCurrenciesAsync(cancellationToken);

        var today = DateTime.UtcNow.Date;
        var ratesCount = 0;

        // کش نرخ‌های امروز برای ارزهای پرکاربرد
        var mainCurrencies = new[] { "USD", "EUR", "IRR" };
        
        foreach (var fromCurrency in currencies.Where(c => mainCurrencies.Contains(c.Code)))
        {
            foreach (var toCurrency in currencies.Where(c => c.Id != fromCurrency.Id))
            {
                var rate = await currencyRepository.GetRateByCodeAsync(
                    fromCurrency.Code, 
                    toCurrency.Code, 
                    today, 
                    cancellationToken);

                if (rate.HasValue)
                {
                    var cacheKey = CacheKeys.CurrencyRate(fromCurrency.Code, toCurrency.Code, today);
                    
                    await _cache.SetStringAsync(
                        cacheKey,
                        rate.Value.ToString(),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                        },
                        cancellationToken);

                    ratesCount++;
                }
            }
        }

        _logger.LogInformation("کش {Count} نرخ ارز به‌روزرسانی شد", ratesCount);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در به‌روزرسانی کش نرخ ارز");
    }
}

/// <summary>
/// به‌روزرسانی کش دسترسی‌های کاربران فعال
/// </summary>
private async Task RefreshActiveUserPermissionsCaches(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    try
    {
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        var auditLogRepository = serviceProvider.GetRequiredService<IAuditLogRepository>();

        // دریافت کاربران فعال در 24 ساعت گذشته
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var activeUserIds = await auditLogRepository
            .GetQueryable()
            .Where(a => a.CreatedAt >= yesterday && a.UserId.HasValue)
            .Select(a => a.UserId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        var permissionsCount = 0;
        foreach (var userId in activeUserIds)
        {
            var permissions = await userRepository.GetUserPermissionsAsync(userId, cancellationToken);
            var cacheKey = CacheKeys.UserPermissions(userId.ToString());
            
            await _cache.SetStringAsync(
                cacheKey,
                permissions.ToJson(),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(CacheKeys.DefaultCacheMinutes)
                },
                cancellationToken);

            permissionsCount++;
        }

        _logger.LogInformation("کش دسترسی‌های {Count} کاربر فعال به‌روزرسانی شد", permissionsCount);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در به‌روزرسانی کش دسترسی‌های کاربران");
    }
}
}
