using System;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت دسترسی
/// </summary>
public class Permission : SoftDeletableEntity
{
    public Guid PermissionGroupId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Code { get; set; } = null!;

    // روابط
    public virtual PermissionGroup PermissionGroup { get; set; } = null!;
}
