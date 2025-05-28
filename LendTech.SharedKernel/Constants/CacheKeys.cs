namespace LendTech.SharedKernel.Constants;

/// <summary>
/// کلیدهای Cache
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// پیشوند کلیدهای کش
    /// </summary>
    private const string Prefix = "LendTech:";

    /// <summary>
    /// کلید کش برای دسترسی‌های کاربر
    /// </summary>
    public static string UserPermissions(string userId) => $"{Prefix}UserPermissions:{userId}";

    /// <summary>
    /// کلید کش برای نقش‌های کاربر
    /// </summary>
    public static string UserRoles(string userId) => $"{Prefix}UserRoles:{userId}";

    /// <summary>
    /// کلید کش برای اطلاعات سازمان
    /// </summary>
    public static string Organization(string organizationId) => $"{Prefix}Organization:{organizationId}";

    /// <summary>
    /// کلید کش برای نرخ ارز
    /// </summary>
    public static string CurrencyRate(string fromCode, string toCode, DateTime date) => 
        $"{Prefix}CurrencyRate:{fromCode}:{toCode}:{date:yyyyMMdd}";

    /// <summary>
    /// کلید کش برای تنظیمات سیستم
    /// </summary>
    public static string SystemSettings => $"{Prefix}SystemSettings";

    /// <summary>
    /// کلید کش برای Feature Flags سازمان
    /// </summary>
    public static string OrganizationFeatures(string organizationId) => 
        $"{Prefix}OrganizationFeatures:{organizationId}";

    /// <summary>
    /// مدت زمان پیش‌فرض کش (دقیقه)
    /// </summary>
    public const int DefaultCacheMinutes = 30;

    /// <summary>
    /// مدت زمان کش کوتاه (دقیقه)
    /// </summary>
    public const int ShortCacheMinutes = 5;

    /// <summary>
    /// مدت زمان کش طولانی (دقیقه)
    /// </summary>
    public const int LongCacheMinutes = 120;
}
