namespace LendTech.SharedKernel.Enums;

/// <summary>
/// وضعیت‌های پاسخ API
/// </summary>
public enum ResponseStatus
{
    /// <summary>
    /// عملیات موفق
    /// </summary>
    Success,

    /// <summary>
    /// خطا در اعتبارسنجی داده‌ها
    /// </summary>
    ValidationError,

    /// <summary>
    /// خطای منطق تجاری
    /// </summary>
    BusinessError,

    /// <summary>
    /// عدم دسترسی
    /// </summary>
    Unauthorized,

    /// <summary>
    /// ممنوعیت دسترسی
    /// </summary>
    Forbidden,

    /// <summary>
    /// یافت نشد
    /// </summary>
    NotFound,

    /// <summary>
    /// خطای سرور
    /// </summary>
    InternalServerError,

    /// <summary>
    /// سرویس در دسترس نیست
    /// </summary>
    ServiceUnavailable
}
