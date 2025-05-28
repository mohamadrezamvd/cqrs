using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای تجاوز از حد مجاز درخواست
/// </summary>
public class RateLimitExceededException : BusinessException
{
    /// <summary>
    /// زمان باقی‌مانده تا رفع محدودیت (ثانیه)
    /// </summary>
    public int RetryAfterSeconds { get; }

    public RateLimitExceededException(int retryAfterSeconds)
        : base($"تعداد درخواست‌ها از حد مجاز گذشته است. لطفاً {retryAfterSeconds} ثانیه دیگر تلاش کنید", "RATE_LIMIT_EXCEEDED")
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}
