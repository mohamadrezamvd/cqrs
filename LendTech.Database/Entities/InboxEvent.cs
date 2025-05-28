using System;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت رویداد Inbox برای دریافت پیام
/// </summary>
public class InboxEvent : BaseEntity
{
    public string MessageId { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string EventData { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public DateTime? ProcessedAt { get; set; }

    // روابط
    public virtual Organization Organization { get; set; } = null!;
}
