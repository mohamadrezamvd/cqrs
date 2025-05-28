using System;
using System.Linq;
using System.Threading.Tasks;
using LendTech.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace LendTech.Database.SeedData;
/// <summary>
/// کلاس مقداردهی اولیه دیتابیس
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// مقداردهی اولیه دیتابیس و ایجاد داده‌های Seed
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LendTechDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LendTechDbContext>>();
        try
        {
            logger.LogInformation("شروع بررسی و ایجاد دیتابیس...");

            // اطمینان از ایجاد دیتابیس
            var dbCreated = await context.Database.EnsureCreatedAsync();

            if (dbCreated)
            {
                logger.LogInformation("دیتابیس جدید ایجاد شد.");
            }
            else
            {
                logger.LogInformation("دیتابیس از قبل وجود دارد.");
            }

            // اجرای Migration های Pending
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("اجرای {Count} Migration در انتظار...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Migration ها با موفقیت اجرا شدند.");
            }

            // بررسی و ایجاد داده‌های Seed
            await SeedDataIfNeededAsync(context, logger);

            logger.LogInformation("مقداردهی اولیه دیتابیس با موفقیت انجام شد.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در مقداردهی اولیه دیتابیس");
            throw;
        }
    }

    /// <summary>
    /// بررسی و ایجاد داده‌های Seed در صورت نیاز
    /// </summary>
    private static async Task SeedDataIfNeededAsync(LendTechDbContext context, ILogger logger)
    {
        // بررسی وجود داده‌های اولیه
        if (await context.Organizations.AnyAsync())
        {
            logger.LogInformation("داده‌های Seed قبلاً ایجاد شده‌اند.");
            return;
        }

        logger.LogInformation("شروع ایجاد داده‌های Seed...");

        // شروع تراکنش برای اطمینان از یکپارچگی داده‌ها
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // سازمان‌ها
            await SeedOrganizationsAsync(context);
            logger.LogInformation("سازمان‌ها ایجاد شدند.");

            // گروه‌های دسترسی
            await SeedPermissionGroupsAsync(context);
            logger.LogInformation("گروه‌های دسترسی ایجاد شدند.");

            // دسترسی‌ها
            await SeedPermissionsAsync(context);
            logger.LogInformation("دسترسی‌ها ایجاد شدند.");

            // نقش‌ها
            await SeedRolesAsync(context);
            logger.LogInformation("نقش‌ها ایجاد شدند.");

            // کاربران
            await SeedUsersAsync(context);
            logger.LogInformation("کاربران ایجاد شدند.");

            // ارتباط کاربران و نقش‌ها
            await SeedUserRolesAsync(context);
            logger.LogInformation("ارتباط کاربران و نقش‌ها ایجاد شد.");

            // ارتباط نقش‌ها و گروه‌های دسترسی
            await SeedRolePermissionGroupsAsync(context);
            logger.LogInformation("ارتباط نقش‌ها و گروه‌های دسترسی ایجاد شد.");

            // ارزها
            await SeedCurrenciesAsync(context);
            logger.LogInformation("ارزها ایجاد شدند.");

            // نرخ ارزها
            await SeedCurrencyRatesAsync(context);
            logger.LogInformation("نرخ ارزها ایجاد شدند.");

            // Commit تراکنش
            await transaction.CommitAsync();
            logger.LogInformation("داده‌های Seed با موفقیت ایجاد شدند.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// ایجاد سازمان‌ها
    /// </summary>
    private static async Task SeedOrganizationsAsync(LendTechDbContext context)
    {
        var organizations = new[]
        {
        new Organization
        {
            Id = DataSeeder.DefaultOrganizationId,
            Name = "سازمان پیش‌فرض",
            Code = "DEFAULT",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            Settings = @"{
                ""general"": {
                    ""timeZone"": ""Asia/Tehran"",
                    ""dateFormat"": ""yyyy/MM/dd"",
                    ""language"": ""fa-IR"",
                    ""theme"": ""default""
                },
                ""financial"": {
                    ""defaultCurrency"": ""IRR"",
                    ""maxTransactionAmount"": 1000000000,
                    ""minTransactionAmount"": 10000
                }
            }",
            Features = @"[
                {""name"": ""UserManagement"", ""isEnabled"": true},
                {""name"": ""FinancialReports"", ""isEnabled"": true},
                {""name"": ""MultiCurrency"", ""isEnabled"": true}
            ]"
        },
        new Organization
        {
            Id = DataSeeder.TestOrganizationId,
            Name = "شرکت آزمایشی",
            Code = "TEST001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        }
    };

        await context.Organizations.AddRangeAsync(organizations);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد گروه‌های دسترسی
    /// </summary>
    private static async Task SeedPermissionGroupsAsync(LendTechDbContext context)
    {
        var permissionGroups = new[]
        {
        new PermissionGroup
        {
            Id = DataSeeder.UserManagementGroupId,
            Name = "مدیریت کاربران",
            Description = "دسترسی‌های مربوط به مدیریت کاربران سیستم",
            IsSystemGroup = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new PermissionGroup
        {
            Id = DataSeeder.RoleManagementGroupId,
            Name = "مدیریت نقش‌ها",
            Description = "دسترسی‌های مربوط به مدیریت نقش‌ها و سطوح دسترسی",
            IsSystemGroup = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new PermissionGroup
        {
            Id = DataSeeder.OrganizationManagementGroupId,
            Name = "مدیریت سازمان",
            Description = "دسترسی‌های مربوط به تنظیمات و مدیریت سازمان",
            IsSystemGroup = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new PermissionGroup
        {
            Id = DataSeeder.FinancialManagementGroupId,
            Name = "مدیریت مالی",
            Description = "دسترسی‌های مربوط به عملیات مالی و تسهیلات",
            IsSystemGroup = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new PermissionGroup
        {
            Id = DataSeeder.ReportManagementGroupId,
            Name = "گزارشات",
            Description = "دسترسی‌های مربوط به مشاهده و تولید گزارشات",
            IsSystemGroup = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        }
    };

        await context.PermissionGroups.AddRangeAsync(permissionGroups);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد دسترسی‌ها
    /// </summary>
    private static async Task SeedPermissionsAsync(LendTechDbContext context)
    {
        var permissions = new List<Permission>();

        // دسترسی‌های مدیریت کاربران
        permissions.AddRange(new[]
        {
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.UserManagementGroupId, Name = "مشاهده کاربران", Code = "Users.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.UserManagementGroupId, Name = "ایجاد کاربر", Code = "Users.Create", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.UserManagementGroupId, Name = "ویرایش کاربر", Code = "Users.Update", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.UserManagementGroupId, Name = "حذف کاربر", Code = "Users.Delete", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.UserManagementGroupId, Name = "تغییر رمز عبور دیگران", Code = "Users.ChangeOthersPassword", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
    });

        // دسترسی‌های مدیریت نقش‌ها
        permissions.AddRange(new[]
        {
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.RoleManagementGroupId, Name = "مشاهده نقش‌ها", Code = "Roles.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.RoleManagementGroupId, Name = "ایجاد نقش", Code = "Roles.Create", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.RoleManagementGroupId, Name = "ویرایش نقش", Code = "Roles.Update", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.RoleManagementGroupId, Name = "حذف نقش", Code = "Roles.Delete", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.RoleManagementGroupId, Name = "مدیریت دسترسی‌ها", Code = "Permissions.Manage", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
    });

        // دسترسی‌های مدیریت سازمان
        permissions.AddRange(new[]
        {
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.OrganizationManagementGroupId, Name = "مشاهده اطلاعات سازمان", Code = "Organization.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.OrganizationManagementGroupId, Name = "ویرایش اطلاعات سازمان", Code = "Organization.Update", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.OrganizationManagementGroupId, Name = "مدیریت تنظیمات", Code = "Organization.ManageSettings", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
    });

        // دسترسی‌های مالی
        permissions.AddRange(new[]
        {
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.FinancialManagementGroupId, Name = "مشاهده تسهیلات", Code = "Loans.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.FinancialManagementGroupId, Name = "مدیریت تسهیلات", Code = "Loans.Manage", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.FinancialManagementGroupId, Name = "تایید تسهیلات", Code = "Loans.Approve", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
    });

        // دسترسی‌های گزارشات
        permissions.AddRange(new[]
        {
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.ReportManagementGroupId, Name = "مشاهده گزارشات مالی", Code = "Financial.ViewReports", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new Permission { Id = Guid.NewGuid(), PermissionGroupId = DataSeeder.ReportManagementGroupId, Name = "دانلود گزارشات", Code = "Reports.Download", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
    });

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد نقش‌ها
    /// </summary>
    private static async Task SeedRolesAsync(LendTechDbContext context)
    {
        var roles = new[]
        {
        new Role
        {
            Id = DataSeeder.SystemAdminRoleId,
            OrganizationId = DataSeeder.DefaultOrganizationId,
            Name = "مدیر سیستم",
            Description = "دسترسی کامل به تمام بخش‌های سیستم",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new Role
        {
            Id = DataSeeder.OrganizationAdminRoleId,
            OrganizationId = DataSeeder.DefaultOrganizationId,
            Name = "مدیر سازمان",
            Description = "مدیریت کامل سازمان و کاربران",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new Role
        {
            Id = DataSeeder.UserRoleId,
            OrganizationId = DataSeeder.DefaultOrganizationId,
            Name = "کاربر عادی",
            Description = "دسترسی‌های پایه کاربری",
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new Role
        {
            Id = DataSeeder.AuditorRoleId,
            OrganizationId = DataSeeder.DefaultOrganizationId,
            Name = "حسابرس",
            Description = "دسترسی مشاهده گزارشات و بررسی عملیات",
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        }
    };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد کاربران
    /// </summary>
    private static async Task SeedUsersAsync(LendTechDbContext context)
    {
        var users = new[]
        {
        new User
        {
            Id = DataSeeder.SystemAdminUserId,
            OrganizationId = DataSeeder.DefaultOrganizationId,
            Username = "admin",
            Email = "admin@lendtech.com",
            PasswordHash = "$2a$11$qFmBBOl2XrXCPx8yZXwAc.KGkYDHLjLnqAHTGhLxuNvZMSfPYGrBm", // رمز: Admin@123
            FirstName = "مدیر",
            LastName = "سیستم",
            MobileNumber = "09121234567",
            IsActive = true,
            IsLocked = false,
            AccessFailedCount = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new User
        {
            Id = DataSeeder.TestAdminUserId,
            OrganizationId = DataSeeder.TestOrganizationId,
            Username = "testadmin",
            Email = "admin@test.com",
            PasswordHash = "$2a$11$qFmBBOl2XrXCPx8yZXwAc.KGkYDHLjLnqAHTGhLxuNvZMSfPYGrBm", // رمز: Admin@123
            FirstName = "مدیر",
            LastName = "آزمایشی",
            MobileNumber = "09129876543",
            IsActive = true,
            IsLocked = false,
            AccessFailedCount = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new User
        {
            Id = DataSeeder.TestUserId,
            OrganizationId = DataSeeder.DefaultOrganizationId,
            Username = "user1",
            Email = "user1@lendtech.com",
            PasswordHash = "$2a$11$qFmBBOl2XrXCPx8yZXwAc.KGkYDHLjLnqAHTGhLxuNvZMSfPYGrBm", // رمز: Admin@123
            FirstName = "کاربر",
            LastName = "آزمایشی",
            MobileNumber = "09121111111",
            IsActive = true,
            IsLocked = false,
            AccessFailedCount = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        }
    };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد ارتباط کاربران و نقش‌ها
    /// </summary>
    private static async Task SeedUserRolesAsync(LendTechDbContext context)
    {
        var userRoles = new[]
        {
        new UserRole
        {
            UserId = DataSeeder.SystemAdminUserId,
            RoleId = DataSeeder.SystemAdminRoleId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new UserRole
        {
            UserId = DataSeeder.TestAdminUserId,
            RoleId = DataSeeder.OrganizationAdminRoleId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new UserRole
        {
            UserId = DataSeeder.TestUserId,
            RoleId = DataSeeder.UserRoleId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        }
    };

        await context.UserRoles.AddRangeAsync(userRoles);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد ارتباط نقش‌ها و گروه‌های دسترسی
    /// </summary>
    private static async Task SeedRolePermissionGroupsAsync(LendTechDbContext context)
    {
        var rolePermissionGroups = new[]
        {
        // مدیر سیستم - دسترسی کامل
        new RolePermissionGroup { RoleId = DataSeeder.SystemAdminRoleId, PermissionGroupId = DataSeeder.UserManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new RolePermissionGroup { RoleId = DataSeeder.SystemAdminRoleId, PermissionGroupId = DataSeeder.RoleManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new RolePermissionGroup { RoleId = DataSeeder.SystemAdminRoleId, PermissionGroupId = DataSeeder.OrganizationManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new RolePermissionGroup { RoleId = DataSeeder.SystemAdminRoleId, PermissionGroupId = DataSeeder.FinancialManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new RolePermissionGroup { RoleId = DataSeeder.SystemAdminRoleId, PermissionGroupId = DataSeeder.ReportManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        
        // مدیر سازمان
        new RolePermissionGroup { RoleId = DataSeeder.OrganizationAdminRoleId, PermissionGroupId = DataSeeder.UserManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new RolePermissionGroup { RoleId = DataSeeder.OrganizationAdminRoleId, PermissionGroupId = DataSeeder.OrganizationManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        new RolePermissionGroup { RoleId = DataSeeder.OrganizationAdminRoleId, PermissionGroupId = DataSeeder.ReportManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        
        // حسابرس
        new RolePermissionGroup { RoleId = DataSeeder.AuditorRoleId, PermissionGroupId = DataSeeder.ReportManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
    };

        await context.RolePermissionGroups.AddRangeAsync(rolePermissionGroups);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد ارزها
    /// </summary>
    private static async Task SeedCurrenciesAsync(LendTechDbContext context)
    {
        var currencies = new[]
        {
        new Currency
        {
            Id = DataSeeder.IrrCurrencyId,
            Code = "IRR",
            Name = "ریال ایران",
            Symbol = "﷼",
            DecimalPlaces = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new Currency
        {
            Id = DataSeeder.UsdCurrencyId,
            Code = "USD",
            Name = "دلار آمریکا",
            Symbol = "$",
            DecimalPlaces = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new Currency
        {
            Id = DataSeeder.EurCurrencyId,
            Code = "EUR",
            Name = "یورو",
            Symbol = "€",
            DecimalPlaces = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        }
    };

        await context.Currencies.AddRangeAsync(currencies);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// ایجاد نرخ ارزها
    /// </summary>
    private static async Task SeedCurrencyRatesAsync(LendTechDbContext context)
    {
        var effectiveDate = DateTime.UtcNow.Date;

        var currencyRates = new[]
        {
        new CurrencyRate
        {
            Id = Guid.NewGuid(),
            FromCurrencyId = DataSeeder.UsdCurrencyId,
            ToCurrencyId = DataSeeder.IrrCurrencyId,
            Rate = 42000,
            EffectiveDate = effectiveDate,
            ExpiryDate = null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new CurrencyRate
        {
            Id = Guid.NewGuid(),
            FromCurrencyId = DataSeeder.EurCurrencyId,
            ToCurrencyId = DataSeeder.IrrCurrencyId,
            Rate = 45500,
            EffectiveDate = effectiveDate,
            ExpiryDate = null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new CurrencyRate
        {
            Id = Guid.NewGuid(),
            FromCurrencyId = DataSeeder.UsdCurrencyId,
            ToCurrencyId = DataSeeder.EurCurrencyId,
            Rate = 0.92m,
            EffectiveDate = effectiveDate,
            ExpiryDate = null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        }
    };

        await context.CurrencyRates.AddRangeAsync(currencyRates);
        await context.SaveChangesAsync();
    }
}
