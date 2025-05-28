using System;
using System.Collections.Generic;
using System.Linq;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای خطاهای اعتبارسنجی
/// </summary>
public class ValidationException : BusinessException
{
    /// <summary>
    /// لیست خطاهای اعتبارسنجی
    /// </summary>
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("یک یا چند خطای اعتبارسنجی رخ داده است", "VALIDATION_ERROR", errors)
    {
        Errors = errors;
    }

    public ValidationException(string field, string message) 
        : base("خطای اعتبارسنجی رخ داده است", "VALIDATION_ERROR", 
            new Dictionary<string, string[]> { { field, new[] { message } } })
    {
        Errors = new Dictionary<string, string[]> { { field, new[] { message } } };
    }

    /// <summary>
    /// تبدیل خطاها به رشته
    /// </summary>
    public override string ToString()
    {
        var errors = Errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}"));
        return $"{Message}\n{string.Join("\n", errors)}";
    }
}
