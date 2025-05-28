using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای خطاهای سیستم پیام‌رسانی
/// </summary>
public class MessagingException : InfrastructureException
{
    /// <summary>
    /// نام صف پیام
    /// </summary>
    public string? QueueName { get; }

    /// <summary>
    /// نوع پیام
    /// </summary>
    public string? MessageType { get; }

    public MessagingException(string message)
        : base(message, "RabbitMQ")
    {
    }

    public MessagingException(string message, string queueName, string? messageType = null)
        : base(message, "RabbitMQ")
    {
        QueueName = queueName;
        MessageType = messageType;
    }

    public MessagingException(string message, Exception innerException)
        : base(message, "RabbitMQ", innerException)
    {
    }
}
