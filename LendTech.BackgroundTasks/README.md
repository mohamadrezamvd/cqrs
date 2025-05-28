LendTech.BackgroundTasks
این لایه شامل Hosted Services برای پردازش‌های پس‌زمینه است.
ساختار پروژه
LendTech.BackgroundTasks/
├── Services/          # Hosted Services
├── Options/          # تنظیمات سرویس‌ها
└── Extensions/       # Extension Methods
سرویس‌های موجود
OutboxDispatcherService

پردازش و ارسال پیام‌های Outbox به RabbitMQ
Retry Policy برای ارسال پیام‌ها
پردازش Batch ای پیام‌ها

InboxProcessorService

دریافت و پردازش پیام‌ها از RabbitMQ
جلوگیری از پردازش تکراری با Inbox Pattern
پردازش انواع مختلف رویدادها

CacheRefreshService

به‌روزرسانی دوره‌ای کش‌های مهم
کش سازمان‌ها و Feature Flags
کش نرخ ارز روزانه
کش دسترسی‌های کاربران فعال

CleanupService

پاکسازی داده‌های قدیمی با Cron Schedule
پاکسازی Audit Logs قدیمی
پاکسازی Outbox/Inbox Events
پاکسازی فایل‌های موقت

تنظیمات (appsettings.json)
json{
  "BackgroundTasks": {
    "Outbox": {
      "ProcessIntervalSeconds": 10,
      "BatchSize": 100,
      "MaxRetryCount": 3
    },
    "Inbox": {
      "UserEventsQueue": "lendtech.user.events",
      "OrganizationEventsQueue": "lendtech.organization.events",
      "FinancialEventsQueue": "lendtech.financial.events",
      "MaxConcurrentMessages": 10
    },
    "CacheRefresh": {
      "RefreshIntervalMinutes": 30,
      "RefreshOrganizationCache": true,
      "RefreshCurrencyRateCache": true,
      "RefreshActiveUserPermissionsCache": true
    },
    "Cleanup": {
      "CronExpression": "0 2 * * *",
      "CleanupAuditLogs": true,
      "AuditLogRetentionDays": 90,
      "CleanupOutboxEvents": true,
      "OutboxEventRetentionDays": 30,
      "CleanupInboxEvents": true,
      "InboxEventRetentionDays": 30,
      "CleanupTempFiles": true,
      "TempFileRetentionDays": 7
    }
  }
}
وابستگی‌ها

Microsoft.Extensions.Hosting 9.0.0
NCrontab 3.3.3 (برای Cron Scheduling)
Polly 8.3.0 (برای Retry Policies)
