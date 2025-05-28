using System;

namespace LendTech.Database.Entities;

/// <summary>
/// موجودیت رابط کاربر و نقش
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    // روابط
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
