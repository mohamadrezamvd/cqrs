using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای خطاهای دامنه (Domain)
/// </summary>
public class DomainException : BusinessException
{
    public DomainException(string message)
        : base(message, "DOMAIN_ERROR")
    {
    }

    public DomainException(string message, string errorCode)
        : base(message, errorCode)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException, "DOMAIN_ERROR")
    {
    }
}
