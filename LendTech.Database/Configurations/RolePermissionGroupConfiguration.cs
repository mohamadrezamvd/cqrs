using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LendTech.Database.Entities;

namespace LendTech.Database.Configurations;

/// <summary>
/// پیکربندی موجودیت رابط نقش و گروه دسترسی
/// </summary>
public class RolePermissionGroupConfiguration : IEntityTypeConfiguration<RolePermissionGroup>
{
    public void Configure(EntityTypeBuilder<RolePermissionGroup> builder)
    {
        builder.ToTable("RolePermissionGroups");

        builder.HasKey(x => new { x.RoleId, x.PermissionGroupId });

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        // روابط
        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissionGroups)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PermissionGroup)
            .WithMany(x => x.RolePermissionGroups)
            .HasForeignKey(x => x.PermissionGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
