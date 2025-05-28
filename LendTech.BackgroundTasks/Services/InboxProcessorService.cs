using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.Infrastructure.RabbitMQ.Interfaces;
using LendTech.BackgroundTasks.Options;
using LendTech.SharedKernel.Extensions;
namespace LendTech.BackgroundTasks.Services;
/// <summary>
/// سرویس پردازش پیام‌های دریافتی از RabbitMQ
/// </summary>
public class InboxProcessorService : BackgroundService
{
private readonly IServiceProvider _serviceProvider;
private readonly ILogger<InboxProcessorService> _logger;
private readonly IMessageConsumer _messageConsumer;
private readonly InboxOptions _options;
public InboxProcessorService(
    IServiceProvider serviceProvider,
    ILogger<InboxProcessorService> logger,
    IMessageConsumer messageConsumer,
    IOptions<InboxOptions> options)
{
    _serviceProvider = serviceProvider;
    _logger = logger;
    _messageConsumer = messageConsumer;
    _options = options.Value;
}

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("سرویس Inbox Processor شروع به کار کرد");

    // ثبت Consumer برای صف‌های مختلف
    await RegisterConsumers(stoppingToken);

    // نگه‌داشتن سرویس تا زمان توقف
    while (!stoppingToken.IsCancellationRequested)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
    }

    _logger.LogInformation("سرویس Inbox Processor متوقف شد");
}

/// <summary>
/// ثبت Consumer ها برای صف‌های مختلف
/// </summary>
private async Task RegisterConsumers(CancellationToken cancellationToken)
{
    // Consumer برای رویدادهای کاربر
    await _messageConsumer.SubscribeAsync(
        _options.UserEventsQueue,
        async (message) => await ProcessMessage(_options.UserEventsQueue, message, cancellationToken),
        cancellationToken);

    // Consumer برای رویدادهای سازمان
    await _messageConsumer.SubscribeAsync(
        _options.OrganizationEventsQueue,
        async (message) => await ProcessMessage(_options.OrganizationEventsQueue, message, cancellationToken),
        cancellationToken);

    // Consumer برای رویدادهای مالی
    await _messageConsumer.SubscribeAsync(
        _options.FinancialEventsQueue,
        async (message) => await ProcessMessage(_options.FinancialEventsQueue, message, cancellationToken),
        cancellationToken);

    _logger.LogInformation("Consumer ها برای صف‌های پیام ثبت شدند");
}

/// <summary>
/// پردازش پیام دریافتی
/// </summary>
private async Task<bool> ProcessMessage(string queueName, string message, CancellationToken cancellationToken)
{
    try
    {
        _logger.LogDebug("پیام دریافت شد از صف {Queue}: {Message}", queueName, message);

        // دیسریالایز پیام
        var messageData = message.FromJson<MessageEnvelope>();
        if (messageData == null)
        {
            _logger.LogWarning("پیام نامعتبر دریافت شد: {Message}", message);
            return false;
        }

        using var scope = _serviceProvider.CreateScope();
        var inboxRepository = scope.ServiceProvider.GetRequiredService<IInboxEventRepository>();

        // بررسی تکراری نبودن پیام
        if (await inboxRepository.IsProcessedAsync(messageData.MessageId, cancellationToken))
        {
            _logger.LogInformation("پیام {MessageId} قبلاً پردازش شده است", messageData.MessageId);
            return true;
        }

        // ثبت پیام در Inbox
        await inboxRepository.RecordEventAsync(
            messageData.MessageId,
            messageData.EventType,
            messageData.EventData,
            messageData.OrganizationId,
            cancellationToken);

        // پردازش پیام بر اساس نوع
        var processed = await ProcessEventByType(messageData, cancellationToken);

        if (processed)
        {
            // علامت‌گذاری به عنوان پردازش شده
            await inboxRepository.MarkAsProcessedAsync(messageData.MessageId, cancellationToken);
            _logger.LogInformation("پیام {MessageId} با موفقیت پردازش شد", messageData.MessageId);
        }

        return processed;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پردازش پیام از صف {Queue}", queueName);
        return false;
    }
}

/// <summary>
/// پردازش رویداد بر اساس نوع
/// </summary>
private async Task<bool> ProcessEventByType(MessageEnvelope message, CancellationToken cancellationToken)
{
    try
    {
        switch (message.EventType)
        {
            case "UserCreated":
                return await ProcessUserCreatedEvent(message.EventData, cancellationToken);
            
            case "UserUpdated":
                return await ProcessUserUpdatedEvent(message.EventData, cancellationToken);
            
            case "RoleAssigned":
                return await ProcessRoleAssignedEvent(message.EventData, cancellationToken);
            
            // سایر انواع رویدادها
            default:
                _logger.LogWarning("نوع رویداد ناشناخته: {EventType}", message.EventType);
                return true; // برای جلوگیری از retry مداوم
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پردازش رویداد {EventType}", message.EventType);
        return false;
    }
}

/// <summary>
/// پردازش رویداد ایجاد کاربر
/// </summary>
private async Task<bool> ProcessUserCreatedEvent(object eventData, CancellationToken cancellationToken)
{
    // TODO: پیاده‌سازی منطق پردازش
    _logger.LogInformation("پردازش رویداد UserCreated");
    await Task.Delay(100, cancellationToken); // شبیه‌سازی پردازش
    return true;
}

/// <summary>
/// پردازش رویداد به‌روزرسانی کاربر
/// </summary>
private async Task<bool> ProcessUserUpdatedEvent(object eventData, CancellationToken cancellationToken)
{
    // TODO: پیاده‌سازی منطق پردازش
    _logger.LogInformation("پردازش رویداد UserUpdated");
    await Task.Delay(100, cancellationToken); // شبیه‌سازی پردازش
    return true;
}

/// <summary>
/// پردازش رویداد اختصاص نقش
/// </summary>
private async Task<bool> ProcessRoleAssignedEvent(object eventData, CancellationToken cancellationToken)
{
    // TODO: پیاده‌سازی منطق پردازش
    _logger.LogInformation("پردازش رویداد RoleAssigned");
    await Task.Delay(100, cancellationToken); // شبیه‌سازی پردازش
    return true;
}
}
/// <summary>
/// پوشش پیام
/// </summary>
public class MessageEnvelope
{
public string MessageId { get; set; } = Guid.NewGuid().ToString();
public string EventType { get; set; } = null!;
public object EventData { get; set; } = null!;
public Guid OrganizationId { get; set; }
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
