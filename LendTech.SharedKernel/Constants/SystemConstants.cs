namespace LendTech.SharedKernel.Constants;

/// <summary>
/// ثابت‌های سیستمی
/// </summary>
public static class SystemConstants
{
    /// <summary>
    /// نام سیستم
    /// </summary>
    public const string SystemName = "LendTech";

    /// <summary>
    /// نسخه API
    /// </summary>
    public const string ApiVersion = "v1";

    /// <summary>
    /// کاربر سیستم (برای عملیات خودکار)
    /// </summary>
    public const string SystemUser = "System";

    /// <summary>
    /// حداکثر تعداد تلاش برای ورود
    /// </summary>
    public const int MaxLoginAttempts = 5;

    /// <summary>
    /// مدت زمان قفل حساب (دقیقه)
    /// </summary>
    public const int AccountLockoutMinutes = 30;

    /// <summary>
    /// حداکثر اندازه فایل آپلود (مگابایت)
    /// </summary>
    public const int MaxUploadSizeMB = 10;

    /// <summary>
    /// مدت زمان اعتبار توکن (ساعت)
    /// </summary>
    public const int TokenExpirationHours = 24;

    /// <summary>
    /// مدت زمان اعتبار Refresh Token (روز)
    /// </summary>
    public const int RefreshTokenExpirationDays = 30;
}
