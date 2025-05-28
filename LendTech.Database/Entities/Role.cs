using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت نقش
/// </summary>
public class Role : SoftDeletableEntity
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

    // روابط
    public virtual Organization Organization { get; set; } = null!;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermissionGroup> RolePermissionGroups { get; set; } = new List<RolePermissionGroup>();
}
