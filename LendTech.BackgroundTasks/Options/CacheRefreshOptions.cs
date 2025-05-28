namespace LendTech.BackgroundTasks.Options;
/// <summary>
/// تنظیمات سرویس به‌روزرسانی کش
/// </summary>
public class CacheRefreshOptions
{
/// <summary>
/// فاصله زمانی به‌روزرسانی (دقیقه)
/// </summary>
public int RefreshIntervalMinutes { get; set; } = 30;
/// <summary>
/// به‌روزرسانی کش سازمان‌ها
/// </summary>
public bool RefreshOrganizationCache { get; set; } = true;

/// <summary>
/// به‌روزرسانی کش نرخ ارز
/// </summary>
public bool RefreshCurrencyRateCache { get; set; } = true;

/// <summary>
/// به‌روزرسانی کش دسترسی‌های کاربران فعال
/// </summary>
public bool RefreshActiveUserPermissionsCache { get; set; } = true;
}
