using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.Infrastructure.RabbitMQ.Interfaces;
using LendTech.BackgroundTasks.Options;
namespace LendTech.BackgroundTasks.Services;
/// <summary>
/// سرویس پردازش و ارسال پیام‌های Outbox
/// </summary>
public class OutboxDispatcherService : BackgroundService
{
private readonly IServiceProvider _serviceProvider;
private readonly ILogger<OutboxDispatcherService> _logger;
private readonly OutboxOptions _options;
private readonly AsyncRetryPolicy _retryPolicy;
public OutboxDispatcherService(
    IServiceProvider serviceProvider,
    ILogger<OutboxDispatcherService> logger,
    IOptions<OutboxOptions> options)
{
    _serviceProvider = serviceProvider;
    _logger = logger;
    _options = options.Value;

    // تنظیم Retry Policy
    _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(
            3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning(exception, 
                    "خطا در پردازش Outbox. تلاش {RetryCount} پس از {TimeSpan} ثانیه", 
                    retryCount, timeSpan.TotalSeconds);
            });
}

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("سرویس Outbox Dispatcher شروع به کار کرد");

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            await ProcessOutboxEvents(stoppingToken);
            
            // تاخیر بین پردازش‌ها
            await Task.Delay(TimeSpan.FromSeconds(_options.ProcessIntervalSeconds), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // خروج از حلقه در صورت لغو
            break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای غیرمنتظره در Outbox Dispatcher");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    _logger.LogInformation("سرویس Outbox Dispatcher متوقف شد");
}

/// <summary>
/// پردازش رویدادهای Outbox
/// </summary>
private async Task ProcessOutboxEvents(CancellationToken cancellationToken)
{
    using var scope = _serviceProvider.CreateScope();
    var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxEventRepository>();
    var messagePublisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

    // دریافت رویدادهای پردازش نشده
    var unprocessedEvents = await outboxRepository.GetUnprocessedEventsAsync(_options.BatchSize, cancellationToken);

    if (unprocessedEvents.Count == 0)
    {
        _logger.LogDebug("هیچ رویداد پردازش نشده‌ای یافت نشد");
        return;
    }

    _logger.LogInformation("تعداد {Count} رویداد برای پردازش یافت شد", unprocessedEvents.Count);

    foreach (var outboxEvent in unprocessedEvents)
    {
        if (cancellationToken.IsCancellationRequested)
            break;

        try
        {
            // ارسال پیام با Retry Policy
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await messagePublisher.PublishAsync(
                    outboxEvent.EventType,
                    outboxEvent.EventData,
                    cancellationToken);
            });

            // علامت‌گذاری به عنوان پردازش شده
            await outboxRepository.MarkAsProcessedAsync(outboxEvent.Id, cancellationToken);
            
            _logger.LogInformation("رویداد {EventId} با موفقیت پردازش شد", outboxEvent.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش رویداد {EventId}", outboxEvent.Id);
            
            // افزایش تعداد تلاش و ثبت خطا
            await outboxRepository.MarkAsFailedAsync(outboxEvent.Id, ex.Message, cancellationToken);
        }
    }
}
}
