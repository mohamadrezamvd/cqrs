using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت سازمان (Tenant)
/// </summary>
public class Organization : SoftDeletableEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; }
    
    /// <summary>
    /// تنظیمات Feature Flags به صورت JSON
    /// </summary>
    public string? Features { get; set; }
    
    /// <summary>
    /// تنظیمات سفارشی سازمان به صورت JSON
    /// </summary>
    public string? Settings { get; set; }

    // روابط
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    public virtual ICollection<OutboxEvent> OutboxEvents { get; set; } = new List<OutboxEvent>();
    public virtual ICollection<InboxEvent> InboxEvents { get; set; } = new List<InboxEvent>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
