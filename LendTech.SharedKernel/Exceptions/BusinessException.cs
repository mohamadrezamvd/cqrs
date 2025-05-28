using System;
using System.Collections.Generic;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception پایه برای خطاهای منطق تجاری
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// کد خطا
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// جزئیات خطا
    /// </summary>
    public Dictionary<string, string[]>? Details { get; }

    public BusinessException(string message, string errorCode = "BUSINESS_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessException(string message, string errorCode, Dictionary<string, string[]> details) 
        : base(message)
    {
        ErrorCode = errorCode;
        Details = details;
    }

    public BusinessException(string message, Exception innerException, string errorCode = "BUSINESS_ERROR") 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
