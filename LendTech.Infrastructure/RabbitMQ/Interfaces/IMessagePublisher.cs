using System.Threading;
using System.Threading.Tasks;
namespace LendTech.Infrastructure.RabbitMQ.Interfaces;
/// <summary>
/// اینترفیس ارسال پیام به RabbitMQ
/// </summary>
public interface IMessagePublisher
{
/// <summary>
/// ارسال پیام
/// </summary>
Task PublishAsync(string eventType, object eventData, CancellationToken cancellationToken = default);
/// <summary>
/// ارسال پیام به صف مشخص
/// </summary>
Task PublishToQueueAsync(string queueName, object message, CancellationToken cancellationToken = default);

/// <summary>
/// ارسال پیام به Exchange
/// </summary>
Task PublishToExchangeAsync(string exchangeName, string routingKey, object message, CancellationToken cancellationToken = default);
}
