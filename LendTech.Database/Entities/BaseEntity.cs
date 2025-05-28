using System;

namespace LendTech.Database.Entities;

/// <summary>
/// کلاس پایه برای تمام موجودیت‌ها
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    
    // فیلدهای ممیزی
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}

/// <summary>
/// کلاس پایه برای موجودیت‌های قابل حذف منطقی
/// </summary>
public abstract class SoftDeletableEntity : BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
