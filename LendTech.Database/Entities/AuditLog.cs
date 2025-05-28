using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

public partial class AuditLog
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid? UserId { get; set; }

    public string EntityName { get; set; } = null!;

    public string? EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual User? User { get; set; }
}
