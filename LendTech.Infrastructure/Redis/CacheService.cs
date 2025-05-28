using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using LendTech.Infrastructure.Redis.Interfaces;
using LendTech.SharedKernel.Extensions;
namespace LendTech.Infrastructure.Redis;
/// <summary>
/// سرویس مدیریت کش با Redis
/// </summary>
public class CacheService : ICacheService
{
private readonly IDistributedCache _cache;
private readonly ILogger<CacheService> _logger;
public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
{
    _cache = cache;
    _logger = logger;
}

/// <inheritdoc />
public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
{
    try
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        
        if (string.IsNullOrEmpty(value))
            return default;

        return value.FromJson<T>();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در دریافت از کش با کلید {Key}", key);
        return default;
    }
}

/// <inheritdoc />
public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
{
    try
    {
        return await _cache.GetStringAsync(key, cancellationToken);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در دریافت رشته از کش با کلید {Key}", key);
        return null;
    }
}

/// <inheritdoc />
public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
{
    try
    {
        var options = new DistributedCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(30); // پیش‌فرض
        }

        var json = value.ToJson();
        await _cache.SetStringAsync(key, json, options, cancellationToken);
        
        _logger.LogDebug("مقدار با کلید {Key} در کش ذخیره شد", key);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ذخیره در کش با کلید {Key}", key);
    }
}

/// <inheritdoc />
public async Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
{
    try
    {
        var options = new DistributedCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(30); // پیش‌فرض
        }

        await _cache.SetStringAsync(key, value, options, cancellationToken);
        
        _logger.LogDebug("رشته با کلید {Key} در کش ذخیره شد", key);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ذخیره رشته در کش با کلید {Key}", key);
    }
}

/// <inheritdoc />
public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
{
    var cachedValue = await GetAsync<T>(key, cancellationToken);
    
    if (cachedValue != null)
        return cachedValue;

    var value = await factory();
    
    if (value != null)
    {
        await SetAsync(key, value, expiration, cancellationToken);
    }

    return value;
}

/// <inheritdoc />
public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
{
    try
    {
        await _cache.RemoveAsync(key, cancellationToken);
        _logger.LogDebug("کلید {Key} از کش حذف شد", key);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در حذف از کش با کلید {Key}", key);
    }
}

/// <inheritdoc />
public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
{
    // Redis نیازمند اسکن کلیدها است که در IDistributedCache موجود نیست
    // برای پیاده‌سازی کامل نیاز به IConnectionMultiplexer است
    _logger.LogWarning("RemoveByPrefixAsync نیازمند پیاده‌سازی مستقیم Redis است");
    await Task.CompletedTask;
}

/// <inheritdoc />
public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
{
    try
    {
        var value = await _cache.GetAsync(key, cancellationToken);
        return value != null;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در بررسی وجود کلید {Key}", key);
        return false;
    }
}

/// <inheritdoc />
public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
{
    try
    {
        await _cache.RefreshAsync(key, cancellationToken);
        _logger.LogDebug("زمان انقضای کلید {Key} تمدید شد", key);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در تمدید زمان انقضای کلید {Key}", key);
    }
}
}
