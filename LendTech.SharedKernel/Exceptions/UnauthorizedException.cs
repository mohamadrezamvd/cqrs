using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای عدم احراز هویت
/// </summary>
public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message = "احراز هویت الزامی است")
        : base(message, "UNAUTHORIZED")
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException, "UNAUTHORIZED")
    {
    }
}
