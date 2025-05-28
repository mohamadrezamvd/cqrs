using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

public partial class InboxEvent
{
    public Guid Id { get; set; }

    public string MessageId { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string EventData { get; set; } = null!;

    public Guid OrganizationId { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
