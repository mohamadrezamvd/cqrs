using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LendTech.Database.Extensions;

/// <summary>
/// Extension های کمکی برای ModelBuilder
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// اعمال قوانین عمومی برای تمام موجودیت‌ها
    /// </summary>
    public static ModelBuilder ApplyGlobalConfigurations(this ModelBuilder modelBuilder)
    {
        // تنظیم رفتار حذف به صورت Restrict برای جلوگیری از Cascade Delete
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // تنظیم نوع داده‌های decimal
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(18);
            property.SetScale(6);
        }

        return modelBuilder;
    }

    /// <summary>
    /// اضافه کردن Seed Data برای داده‌های اولیه سیستم
    /// </summary>
    public static ModelBuilder SeedInitialData(this ModelBuilder modelBuilder)
    {
        // ایجاد سازمان پیش‌فرض
        var defaultOrgId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<Entities.Organization>().HasData(new Entities.Organization
        {
            Id = defaultOrgId,
            Name = "سازمان پیش‌فرض",
            Code = "DEFAULT",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        // ایجاد گروه‌های دسترسی پیش‌فرض
        var userManagementGroupId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var roleManagementGroupId = Guid.Parse("00000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<Entities.PermissionGroup>().HasData(
            new Entities.PermissionGroup
            {
                Id = userManagementGroupId,
                Name = "مدیریت کاربران",
                Description = "دسترسی‌های مربوط به مدیریت کاربران",
                IsSystemGroup = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.PermissionGroup
            {
                Id = roleManagementGroupId,
                Name = "مدیریت نقش‌ها",
                Description = "دسترسی‌های مربوط به مدیریت نقش‌ها",
                IsSystemGroup = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );

        // ایجاد دسترسی‌های پیش‌فرض
        modelBuilder.Entity<Entities.Permission>().HasData(
            // دسترسی‌های مدیریت کاربران
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = userManagementGroupId,
                Name = "مشاهده کاربران",
                Code = "Users.View",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = userManagementGroupId,
                Name = "ایجاد کاربر",
                Code = "Users.Create",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = userManagementGroupId,
                Name = "ویرایش کاربر",
                Code = "Users.Update",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = userManagementGroupId,
                Name = "حذف کاربر",
                Code = "Users.Delete",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            // دسترسی‌های مدیریت نقش‌ها
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = roleManagementGroupId,
                Name = "مشاهده نقش‌ها",
                Code = "Roles.View",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = roleManagementGroupId,
                Name = "ایجاد نقش",
                Code = "Roles.Create",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = roleManagementGroupId,
                Name = "ویرایش نقش",
                Code = "Roles.Update",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Permission
            {
                Id = Guid.NewGuid(),
                PermissionGroupId = roleManagementGroupId,
                Name = "حذف نقش",
                Code = "Roles.Delete",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );

        // ایجاد نقش مدیر سیستم
        var adminRoleId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        modelBuilder.Entity<Entities.Role>().HasData(new Entities.Role
        {
            Id = adminRoleId,
            OrganizationId = defaultOrgId,
            Name = "مدیر سیستم",
            Description = "دسترسی کامل به تمام بخش‌های سیستم",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });

        // اتصال نقش مدیر به گروه‌های دسترسی
        modelBuilder.Entity<Entities.RolePermissionGroup>().HasData(
            new Entities.RolePermissionGroup
            {
                RoleId = adminRoleId,
                PermissionGroupId = userManagementGroupId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.RolePermissionGroup
            {
                RoleId = adminRoleId,
                PermissionGroupId = roleManagementGroupId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );

        // ایجاد ارزهای پیش‌فرض
        modelBuilder.Entity<Entities.Currency>().HasData(
            new Entities.Currency
            {
                Id = Guid.NewGuid(),
                Code = "IRR",
                Name = "ریال ایران",
                Symbol = "﷼",
                DecimalPlaces = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Currency
            {
                Id = Guid.NewGuid(),
                Code = "USD",
                Name = "دلار آمریکا",
                Symbol = "$",
                DecimalPlaces = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Entities.Currency
            {
                Id = Guid.NewGuid(),
                Code = "EUR",
                Name = "یورو",
                Symbol = "€",
                DecimalPlaces = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );

        return modelBuilder;
    }
}
