using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت گروه دسترسی
/// </summary>
public class PermissionGroup : SoftDeletableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemGroup { get; set; }

    // روابط
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    public virtual ICollection<RolePermissionGroup> RolePermissionGroups { get; set; } = new List<RolePermissionGroup>();
}
