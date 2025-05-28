namespace LendTech.SharedKernel.Enums;

/// <summary>
/// انواع عملیات برای ثبت در Audit Log
/// </summary>
public enum AuditAction
{
    /// <summary>
    /// ایجاد رکورد جدید
    /// </summary>
    Create,

    /// <summary>
    /// به‌روزرسانی رکورد
    /// </summary>
    Update,

    /// <summary>
    /// حذف رکورد
    /// </summary>
    Delete,

    /// <summary>
    /// مشاهده رکورد
    /// </summary>
    Read,

    /// <summary>
    /// ورود به سیستم
    /// </summary>
    Login,

    /// <summary>
    /// خروج از سیستم
    /// </summary>
    Logout,

    /// <summary>
    /// تلاش ناموفق برای ورود
    /// </summary>
    LoginFailed,

    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    PasswordChanged,

    /// <summary>
    /// قفل شدن حساب کاربری
    /// </summary>
    AccountLocked,

    /// <summary>
    /// باز شدن قفل حساب کاربری
    /// </summary>
    AccountUnlocked
}
