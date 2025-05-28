using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای خطاهای ارتباط با سرویس‌های خارجی
/// </summary>
public class IntegrationException : InfrastructureException
{
    /// <summary>
    /// کد وضعیت HTTP (در صورت وجود)
    /// </summary>
    public int? HttpStatusCode { get; }

    /// <summary>
    /// پاسخ سرویس خارجی
    /// </summary>
    public string? ResponseContent { get; }

    public IntegrationException(string message, string serviceName)
        : base(message, serviceName)
    {
    }

    public IntegrationException(string message, string serviceName, int httpStatusCode, string? responseContent = null)
        : base(message, serviceName)
    {
        HttpStatusCode = httpStatusCode;
        ResponseContent = responseContent;
    }

    public IntegrationException(string message, string serviceName, Exception innerException)
        : base(message, serviceName, innerException)
    {
    }
}
