using System;
using LendTech.SharedKernel.Enums;

namespace LendTech.SharedKernel.Models;

/// <summary>
/// مدل پایه رویداد Integration
/// </summary>
public abstract class IntegrationEvent
{
    /// <summary>
    /// شناسه رویداد
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// نوع رویداد
    /// </summary>
    public abstract EventType EventType { get; }

    /// <summary>
    /// زمان ایجاد رویداد
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// شناسه سازمان
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// شناسه کاربر ایجاد کننده
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// داده‌های اضافی
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}
