using System;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت رویداد Outbox برای ارسال پیام
/// </summary>
public class OutboxEvent : BaseEntity
{
    public string EventType { get; set; } = null!;
    public string EventData { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; }

    // روابط
    public virtual Organization Organization { get; set; } = null!;
}
