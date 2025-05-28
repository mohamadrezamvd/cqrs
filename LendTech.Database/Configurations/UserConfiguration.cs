using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LendTech.Database.Entities;

namespace LendTech.Database.Configurations;

/// <summary>
/// پیکربندی موجودیت کاربر
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(x => x.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        builder.Property(x => x.MobileNumber)
            .HasMaxLength(20);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.IsLocked)
            .HasDefaultValue(false);

        builder.Property(x => x.AccessFailedCount)
            .HasDefaultValue(0);

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

        // ایندکس‌ها
        builder.HasIndex(x => x.OrganizationId);

        builder.HasIndex(x => new { x.Username, x.OrganizationId })
            .IsUnique();

        builder.HasIndex(x => new { x.Email, x.OrganizationId })
            .IsUnique();

        // روابط
        builder.HasOne(x => x.Organization)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // فیلتر کوئری برای حذف منطقی
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
