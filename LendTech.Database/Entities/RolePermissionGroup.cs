using System;
using System.Collections.Generic;

namespace LendTech.Database.Entities;

public partial class RolePermissionGroup
{
    public Guid RoleId { get; set; }

    public Guid PermissionGroupId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual PermissionGroup PermissionGroup { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
