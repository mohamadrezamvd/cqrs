namespace LendTech.BackgroundTasks.Options;
/// <summary>
/// تنظیمات سرویس Inbox
/// </summary>
public class InboxOptions
{
/// <summary>
/// نام صف رویدادهای کاربر
/// </summary>
public string UserEventsQueue { get; set; } = "lendtech.user.events";
/// <summary>
/// نام صف رویدادهای سازمان
/// </summary>
public string OrganizationEventsQueue { get; set; } = "lendtech.organization.events";

/// <summary>
/// نام صف رویدادهای مالی
/// </summary>
public string FinancialEventsQueue { get; set; } = "lendtech.financial.events";

/// <summary>
/// حداکثر تعداد پیام همزمان
/// </summary>
public int MaxConcurrentMessages { get; set; } = 10;
}
