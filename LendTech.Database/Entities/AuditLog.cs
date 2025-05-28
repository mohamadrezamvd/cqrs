using System;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت لاگ ممیزی
/// </summary>
public class AuditLog : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Guid? UserId { get; set; }
    public string EntityName { get; set; } = null!;
    public string? EntityId { get; set; }
    public string Action { get; set; } = null!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // روابط
    public virtual Organization Organization { get; set; } = null!;
    public virtual User? User { get; set; }
}
