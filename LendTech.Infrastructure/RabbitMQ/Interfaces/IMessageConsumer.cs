using System;
using System.Threading;
using System.Threading.Tasks;
namespace LendTech.Infrastructure.RabbitMQ.Interfaces;
/// <summary>
/// اینترفیس دریافت پیام از RabbitMQ
/// </summary>
public interface IMessageConsumer
{
/// <summary>
/// ثبت Consumer برای صف
/// </summary>
Task SubscribeAsync(string queueName, Func<string, Task<bool>> messageHandler, CancellationToken cancellationToken = default);
/// <summary>
/// لغو اشتراک
/// </summary>
Task UnsubscribeAsync(string queueName, CancellationToken cancellationToken = default);
}
