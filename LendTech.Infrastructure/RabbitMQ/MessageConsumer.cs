using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using LendTech.Infrastructure.RabbitMQ.Interfaces;
using LendTech.Infrastructure.RabbitMQ.Options;
namespace LendTech.Infrastructure.RabbitMQ;
/// <summary>
/// پیاده‌سازی دریافت پیام از RabbitMQ
/// </summary>
public class MessageConsumer : IMessageConsumer, IDisposable
{
private readonly ILogger<MessageConsumer> _logger;
private readonly RabbitMQOptions _options;
private readonly IConnection _connection;
private readonly IModel _channel;
private readonly ConcurrentDictionary<string, EventingBasicConsumer> _consumers;
public MessageConsumer(
    ILogger<MessageConsumer> logger,
    IOptions<RabbitMQOptions> options)
{
    _logger = logger;
    _options = options.Value;
    _consumers = new ConcurrentDictionary<string, EventingBasicConsumer>();

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
}

/// <inheritdoc />
public async Task SubscribeAsync(string queueName, Func<string, Task<bool>> messageHandler, CancellationToken cancellationToken = default)
{
    try
    {
        // بررسی وجود Queue
        _channel.QueueDeclarePassive(queueName);

        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            _logger.LogDebug("پیام دریافت شد از صف {Queue}", queueName);

            try
            {
                var success = await messageHandler(message);
                
                if (success)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogDebug("پیام با موفقیت پردازش شد");
                }
                else
                {
                    // Requeue پیام در صورت عدم موفقیت
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                    _logger.LogWarning("پیام مجدداً به صف برگردانده شد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پردازش پیام");
                
                // ارسال به Dead Letter Queue
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        var consumerTag = _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        _consumers[queueName] = consumer;
        
        _logger.LogInformation("Consumer برای صف {Queue} با تگ {Tag} ثبت شد", queueName, consumerTag);
        
        await Task.CompletedTask;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ثبت Consumer برای صف {Queue}", queueName);
        throw;
    }
}

/// <inheritdoc />
public async Task UnsubscribeAsync(string queueName, CancellationToken cancellationToken = default)
{
    try
    {
        if (_consumers.TryRemove(queueName, out var consumer))
        {
            foreach (var tag in consumer.ConsumerTags)
            {
                _channel.BasicCancel(tag);
            }
            
            _logger.LogInformation("Consumer از صف {Queue} حذف شد", queueName);
        }
        
        await Task.CompletedTask;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در حذف Consumer از صف {Queue}", queueName);
        throw;
    }
}

public void Dispose()
{
    // حذف همه Consumer ها
    foreach (var consumer in _consumers.Values)
    {
        foreach (var tag in consumer.ConsumerTags)
        {
            try
            {
                _channel.BasicCancel(tag);
            }
            catch { }
        }
    }

    _channel?.Dispose();
    _connection?.Dispose();
}
}
