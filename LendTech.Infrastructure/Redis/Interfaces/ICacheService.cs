using System;
using System.Threading;
using System.Threading.Tasks;
namespace LendTech.Infrastructure.Redis.Interfaces;
/// <summary>
/// اینترفیس سرویس کش
/// </summary>
public interface ICacheService
{
/// <summary>
/// دریافت مقدار از کش
/// </summary>
Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
/// <summary>
/// دریافت رشته از کش
/// </summary>
Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

/// <summary>
/// ذخیره مقدار در کش
/// </summary>
Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

/// <summary>
/// ذخیره رشته در کش
/// </summary>
Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

/// <summary>
/// دریافت یا ایجاد مقدار در کش
/// </summary>
Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

/// <summary>
/// حذف از کش
/// </summary>
Task RemoveAsync(string key, CancellationToken cancellationToken = default);

/// <summary>
/// حذف کلیدها با پیشوند
/// </summary>
Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

/// <summary>
/// بررسی وجود کلید
/// </summary>
Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

/// <summary>
/// تمدید زمان انقضا
/// </summary>
Task RefreshAsync(string key, CancellationToken cancellationToken = default);
}
