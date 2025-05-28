using System;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت رابط نقش و گروه دسترسی
/// </summary>
public class RolePermissionGroup
{
    public Guid RoleId { get; set; }
    public Guid PermissionGroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    // روابط
    public virtual Role Role { get; set; } = null!;
    public virtual PermissionGroup PermissionGroup { get; set; } = null!;
}
