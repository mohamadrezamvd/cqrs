using System;

namespace LendTech.SharedKernel.Models;

/// <summary>
/// مدل اطلاعات Audit
/// </summary>
public class AuditInfo
{
    /// <summary>
    /// نام موجودیت
    /// </summary>
    public string EntityName { get; set; } = null!;

    /// <summary>
    /// شناسه موجودیت
    /// </summary>
    public string EntityId { get; set; } = null!;

    /// <summary>
    /// نوع عملیات
    /// </summary>
    public string Action { get; set; } = null!;

    /// <summary>
    /// کاربر انجام دهنده
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// نام کاربر
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// آدرس IP
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// زمان عملیات
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// مقادیر قبلی
    /// </summary>
    public Dictionary<string, object>? OldValues { get; set; }

    /// <summary>
    /// مقادیر جدید
    /// </summary>
    public Dictionary<string, object>? NewValues { get; set; }
}
