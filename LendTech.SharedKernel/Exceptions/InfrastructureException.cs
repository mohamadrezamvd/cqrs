using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای خطاهای زیرساخت
/// </summary>
public class InfrastructureException : Exception
{
    /// <summary>
    /// نام سرویس
    /// </summary>
    public string? ServiceName { get; }

    public InfrastructureException(string message)
        : base(message)
    {
    }

    public InfrastructureException(string message, string serviceName)
        : base(message)
    {
        ServiceName = serviceName;
    }

    public InfrastructureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public InfrastructureException(string message, string serviceName, Exception innerException)
        : base(message, innerException)
    {
        ServiceName = serviceName;
    }
}
