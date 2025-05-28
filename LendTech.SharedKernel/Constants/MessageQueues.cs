namespace LendTech.SharedKernel.Constants;

/// <summary>
/// نام صف‌های پیام RabbitMQ
/// </summary>
public static class MessageQueues
{
    /// <summary>
    /// صف رویدادهای کاربر
    /// </summary>
    public const string UserEvents = "lendtech.user.events";

    /// <summary>
    /// صف رویدادهای سازمان
    /// </summary>
    public const string OrganizationEvents = "lendtech.organization.events";

    /// <summary>
    /// صف رویدادهای مالی
    /// </summary>
    public const string FinancialEvents = "lendtech.financial.events";

    /// <summary>
    /// صف ایمیل
    /// </summary>
    public const string EmailQueue = "lendtech.email.queue";

    /// <summary>
    /// صف پیامک
    /// </summary>
    public const string SmsQueue = "lendtech.sms.queue";

    /// <summary>
    /// صف نوتیفیکیشن
    /// </summary>
    public const string NotificationQueue = "lendtech.notification.queue";

    /// <summary>
    /// صف لاگ
    /// </summary>
    public const string LogQueue = "lendtech.log.queue";

    /// <summary>
    /// Exchange پیش‌فرض
    /// </summary>
    public const string DefaultExchange = "lendtech.exchange";

    /// <summary>
    /// Dead Letter Exchange
    /// </summary>
    public const string DeadLetterExchange = "lendtech.dlx";

    /// <summary>
    /// Dead Letter Queue
    /// </summary>
    public const string DeadLetterQueue = "lendtech.dlq";
}
