namespace LendTech.BackgroundTasks.Options;
/// <summary>
/// تنظیمات سرویس پاکسازی
/// </summary>
public class CleanupOptions
{
	/// <summary>
	/// Cron Expression برای زمان‌بندی
	/// پیش‌فرض: هر روز ساعت 2 صبح
	/// </summary>
	public string CronExpression { get; set; } = "0 2 * * *";
	/// <summary>
	/// پاکسازی لاگ‌های ممیزی
	/// </summary>
	public bool CleanupAuditLogs { get; set; } = true;

	/// <summary>
	/// مدت نگهداری لاگ‌های ممیزی (روز)
	/// </summary>
	public int AuditLogRetentionDays { get; set; } = 90;

	/// <summary>
	/// پاکسازی رویدادهای Outbox
	/// </summary>
	public bool CleanupOutboxEvents { get; set; } = true;

	/// <summary>
	/// مدت نگهداری رویدادهای Outbox (روز)
	/// </summary>
	public int OutboxEventRetentionDays { get; set; } = 30;

	/// <summary>
	/// پاکسازی رویدادهای Inbox
	/// </summary>
	public bool CleanupInboxEvents { get; set; } = true;

	/// <summary>
	/// مدت نگهداری رویدادهای Inbox (روز)
	/// </summary>
	public int InboxEventRetentionDays { get; set; } = 30;

	/// <summary>
	/// پاکسازی فایل‌های موقت
	/// </summary>
	public bool CleanupTempFiles { get; set; } = true;

	/// <summary>
	/// مدت نگهداری فایل‌های موقت (روز)
	/// </summary>
	public int TempFileRetentionDays { get; set; } = 7;
}
