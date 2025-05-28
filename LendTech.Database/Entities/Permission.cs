using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

public partial class Permission
{
    public Guid Id { get; set; }

    public Guid PermissionGroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Code { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual PermissionGroup PermissionGroup { get; set; } = null!;
}
