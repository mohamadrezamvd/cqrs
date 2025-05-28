using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using LendTech.Infrastructure.RabbitMQ.Interfaces;
using LendTech.Infrastructure.RabbitMQ.Options;
using LendTech.SharedKernel.Extensions;
using LendTech.SharedKernel.Constants;
namespace LendTech.Infrastructure.RabbitMQ;
/// <summary>
/// پیاده‌سازی ارسال پیام به RabbitMQ
/// </summary>
public class MessagePublisher : IMessagePublisher, IDisposable
{
private readonly ILogger<MessagePublisher> _logger;
private readonly RabbitMQOptions _options;
private readonly IConnection _connection;
private readonly IModel _channel;
public MessagePublisher(
    ILogger<MessagePublisher> logger,
    IOptions<RabbitMQOptions> options)
{
    _logger = logger;
    _options = options.Value;

    // ایجاد اتصال
    var factory = new ConnectionFactory
    {
        HostName = _options.HostName,
        Port = _options.Port,
        UserName = _options.UserName,
        Password = _options.Password,
        VirtualHost = _options.VirtualHost,
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
    };

    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();

    // تنظیمات کانال
    _channel.BasicQos(0, _options.PrefetchCount, false);

    // ایجاد Exchange ها
    DeclareExchanges();
}

/// <inheritdoc />
public async Task PublishAsync(string eventType, object eventData, CancellationToken cancellationToken = default)
{
    var queueName = GetQueueNameForEventType(eventType);
    await PublishToQueueAsync(queueName, new
    {
        EventType = eventType,
        EventData = eventData,
        MessageId = Guid.NewGuid().ToString(),
        CreatedAt = DateTime.UtcNow
    }, cancellationToken);
}

/// <inheritdoc />
public async Task PublishToQueueAsync(string queueName, object message, CancellationToken cancellationToken = default)
{
    await PublishToExchangeAsync(MessageQueues.DefaultExchange, queueName, message, cancellationToken);
}

/// <inheritdoc />
public async Task PublishToExchangeAsync(string exchangeName, string routingKey, object message, CancellationToken cancellationToken = default)
{
    try
    {
        var json = message.ToJson();
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.MessageId = Guid.NewGuid().ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        await Task.Run(() =>
        {
            lock (_channel)
            {
                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body);
            }
        }, cancellationToken);

        _logger.LogDebug("پیام به {Exchange} با کلید {RoutingKey} ارسال شد", exchangeName, routingKey);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ارسال پیام به {Exchange}", exchangeName);
        throw;
    }
}

/// <summary>
/// ایجاد Exchange ها و Queue ها
/// </summary>
private void DeclareExchanges()
{
    try
    {
        // Exchange اصلی
        _channel.ExchangeDeclare(
            exchange: MessageQueues.DefaultExchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        // Dead Letter Exchange
        _channel.ExchangeDeclare(
            exchange: MessageQueues.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // Queue ها
        DeclareQueue(MessageQueues.UserEvents);
        DeclareQueue(MessageQueues.OrganizationEvents);
        DeclareQueue(MessageQueues.FinancialEvents);
        DeclareQueue(MessageQueues.EmailQueue);
        DeclareQueue(MessageQueues.SmsQueue);
        DeclareQueue(MessageQueues.NotificationQueue);
        DeclareQueue(MessageQueues.LogQueue);

        // Dead Letter Queue
        _channel.QueueDeclare(
            queue: MessageQueues.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: MessageQueues.DeadLetterQueue,
            exchange: MessageQueues.DeadLetterExchange,
            routingKey: MessageQueues.DeadLetterQueue);

        _logger.LogInformation("Exchange ها و Queue های RabbitMQ ایجاد شدند");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ایجاد Exchange ها و Queue ها");
        throw;
    }
}

/// <summary>
/// ایجاد Queue
/// </summary>
private void DeclareQueue(string queueName)
{
    var args = new Dictionary<string, object>
    {
        { "x-dead-letter-exchange", MessageQueues.DeadLetterExchange },
        { "x-dead-letter-routing-key", MessageQueues.DeadLetterQueue },
        { "x-message-ttl", _options.MessageTTL }
    };

    _channel.QueueDeclare(
        queue: queueName,
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: args);

    _channel.QueueBind(
        queue: queueName,
        exchange: MessageQueues.DefaultExchange,
        routingKey: queueName);
}

/// <summary>
/// تعیین نام صف بر اساس نوع رویداد
/// </summary>
private string GetQueueNameForEventType(string eventType)
{
    return eventType switch
    {
        var t when t.StartsWith("User") => MessageQueues.UserEvents,
        var t when t.StartsWith("Organization") => MessageQueues.OrganizationEvents,
        var t when t.StartsWith("Financial") || t.StartsWith("Loan") || t.StartsWith("Payment") => MessageQueues.FinancialEvents,
        _ => MessageQueues.DefaultExchange
    };
}

public void Dispose()
{
    _channel?.Dispose();
    _connection?.Dispose();
}
}
