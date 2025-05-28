using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای تداخل در عملیات
/// </summary>
public class ConflictException : BusinessException
{
    public ConflictException(string message)
        : base(message, "CONFLICT")
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException, "CONFLICT")
    {
    }
}
