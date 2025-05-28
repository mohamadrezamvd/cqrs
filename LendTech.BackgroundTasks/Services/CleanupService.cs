using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.BackgroundTasks.Options;
namespace LendTech.BackgroundTasks.Services;
/// <summary>
/// سرویس پاکسازی داده‌های قدیمی
/// </summary>
public class CleanupService : BackgroundService
{
private readonly IServiceProvider _serviceProvider;
private readonly ILogger<CleanupService> _logger;
private readonly CleanupOptions _options;
private readonly CrontabSchedule _schedule;
public CleanupService(
    IServiceProvider serviceProvider,
    ILogger<CleanupService> logger,
    IOptions<CleanupOptions> options)
{
    _serviceProvider = serviceProvider;
    _logger = logger;
    _options = options.Value;
    
    // پارس کردن Cron Expression
    _schedule = CrontabSchedule.Parse(_options.CronExpression);
}

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("سرویس Cleanup شروع به کار کرد با زمان‌بندی: {Cron}", _options.CronExpression);

    while (!stoppingToken.IsCancellationRequested)
    {
        var now = DateTime.UtcNow;
        var nextRun = _schedule.GetNextOccurrence(now);
        var delay = nextRun - now;

        if (delay > TimeSpan.Zero)
        {
            _logger.LogInformation("اجرای بعدی Cleanup در: {NextRun}", nextRun);
            await Task.Delay(delay, stoppingToken);
        }

        if (!stoppingToken.IsCancellationRequested)
        {
            await RunCleanupTasks(stoppingToken);
        }
    }

    _logger.LogInformation("سرویس Cleanup متوقف شد");
}

/// <summary>
/// اجرای وظایف پاکسازی
/// </summary>
private async Task RunCleanupTasks(CancellationToken cancellationToken)
{
    _logger.LogInformation("شروع اجرای وظایف پاکسازی");

    try
    {
        using var scope = _serviceProvider.CreateScope();

        // پاکسازی Audit Logs
        if (_options.CleanupAuditLogs)
        {
            await CleanupAuditLogs(scope.ServiceProvider, cancellationToken);
        }

        // پاکسازی Outbox Events
        if (_options.CleanupOutboxEvents)
        {
            await CleanupOutboxEvents(scope.ServiceProvider, cancellationToken);
        }

        // پاکسازی Inbox Events
        if (_options.CleanupInboxEvents)
        {
            await CleanupInboxEvents(scope.ServiceProvider, cancellationToken);
        }

        // پاکسازی فایل‌های موقت
        if (_options.CleanupTempFiles)
        {
            await CleanupTempFiles(cancellationToken);
        }

        _logger.LogInformation("وظایف پاکسازی با موفقیت کامل شد");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در اجرای وظایف پاکسازی");
    }
}

/// <summary>
/// پاکسازی لاگ‌های ممیزی قدیمی
/// </summary>
private async Task CleanupAuditLogs(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    try
    {
        var auditLogRepository = serviceProvider.GetRequiredService<IAuditLogRepository>();
        var deletedCount = await auditLogRepository.CleanupOldLogsAsync(_options.AuditLogRetentionDays, cancellationToken);
        
        _logger.LogInformation("تعداد {Count} لاگ ممیزی قدیمی پاک شد", deletedCount);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پاکسازی لاگ‌های ممیزی");
    }
}

/// <summary>
/// پاکسازی رویدادهای Outbox قدیمی
/// </summary>
private async Task CleanupOutboxEvents(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    try
    {
        var outboxRepository = serviceProvider.GetRequiredService<IOutboxEventRepository>();
        var deletedCount = await outboxRepository.CleanupOldEventsAsync(_options.OutboxEventRetentionDays, cancellationToken);
        
        _logger.LogInformation("تعداد {Count} رویداد Outbox قدیمی پاک شد", deletedCount);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پاکسازی رویدادهای Outbox");
    }
}

/// <summary>
/// پاکسازی رویدادهای Inbox قدیمی
/// </summary>
private async Task CleanupInboxEvents(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    try
    {
        var inboxRepository = serviceProvider.GetRequiredService<IInboxEventRepository>();
        var deletedCount = await inboxRepository.CleanupOldEventsAsync(_options.InboxEventRetentionDays, cancellationToken);
        
        _logger.LogInformation("تعداد {Count} رویداد Inbox قدیمی پاک شد", deletedCount);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پاکسازی رویدادهای Inbox");
    }
}

/// <summary>
/// پاکسازی فایل‌های موقت
/// </summary>
private async Task CleanupTempFiles(CancellationToken cancellationToken)
{
    try
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "LendTech");
        
        if (!Directory.Exists(tempPath))
            return;

        var cutoffDate = DateTime.UtcNow.AddDays(-_options.TempFileRetentionDays);
        var files = Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories);
        var deletedCount = 0;

        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var fileInfo = new FileInfo(file);
            if (fileInfo.LastWriteTimeUtc < cutoffDate)
            {
                try
                {
                    File.Delete(file);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "نتوانستیم فایل {File} را پاک کنیم", file);
                }
            }
        }

        _logger.LogInformation("تعداد {Count} فایل موقت پاک شد", deletedCount);
        await Task.CompletedTask;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پاکسازی فایل‌های موقت");
    }
}
}
