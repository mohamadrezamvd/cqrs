using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LendTech.Database.Entities;

namespace LendTech.Database.Configurations;

/// <summary>
/// پیکربندی موجودیت دسترسی
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        // روابط
        builder.HasOne(x => x.PermissionGroup)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.PermissionGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // فیلتر کوئری برای حذف منطقی
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
