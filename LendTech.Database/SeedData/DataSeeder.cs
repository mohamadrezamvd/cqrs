using LendTech.Database.Entities;
using Microsoft.EntityFrameworkCore;
namespace LendTech.Database.SeedData;
/// <summary>
/// کلاس Seed کردن داده‌های اولیه سیستم
/// </summary>
public static class DataSeeder
{
	// شناسه‌های ثابت برای سازمان‌ها
	public static readonly Guid DefaultOrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
	public static readonly Guid TestOrganizationId = Guid.Parse("00000000-0000-0000-0000-000000000002");
	// شناسه‌های ثابت برای گروه‌های دسترسی
	public static readonly Guid UserManagementGroupId = Guid.Parse("10000000-0000-0000-0000-000000000001");
	public static readonly Guid RoleManagementGroupId = Guid.Parse("10000000-0000-0000-0000-000000000002");
	public static readonly Guid OrganizationManagementGroupId = Guid.Parse("10000000-0000-0000-0000-000000000003");
	public static readonly Guid FinancialManagementGroupId = Guid.Parse("10000000-0000-0000-0000-000000000004");
	public static readonly Guid ReportManagementGroupId = Guid.Parse("10000000-0000-0000-0000-000000000005");

	// شناسه‌های ثابت برای نقش‌ها
	public static readonly Guid SystemAdminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
	public static readonly Guid OrganizationAdminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000002");
	public static readonly Guid UserRoleId = Guid.Parse("20000000-0000-0000-0000-000000000003");
	public static readonly Guid AuditorRoleId = Guid.Parse("20000000-0000-0000-0000-000000000004");

	// شناسه‌های ثابت برای کاربران
	public static readonly Guid SystemAdminUserId = Guid.Parse("30000000-0000-0000-0000-000000000001");
	public static readonly Guid TestAdminUserId = Guid.Parse("30000000-0000-0000-0000-000000000002");
	public static readonly Guid TestUserId = Guid.Parse("30000000-0000-0000-0000-000000000003");

	// شناسه‌های ثابت برای ارزها
	public static readonly Guid IrrCurrencyId = Guid.Parse("40000000-0000-0000-0000-000000000001");
	public static readonly Guid UsdCurrencyId = Guid.Parse("40000000-0000-0000-0000-000000000002");
	public static readonly Guid EurCurrencyId = Guid.Parse("40000000-0000-0000-0000-000000000003");

	/// <summary>
	/// اعمال Seed Data به ModelBuilder
	/// </summary>
	public static void Seed(ModelBuilder modelBuilder)
	{
		SeedOrganizations(modelBuilder);
		SeedPermissionGroups(modelBuilder);
		SeedPermissions(modelBuilder);
		SeedRoles(modelBuilder);
		SeedUsers(modelBuilder);
		SeedUserRoles(modelBuilder);
		SeedRolePermissionGroups(modelBuilder);
		SeedCurrencies(modelBuilder);
		SeedCurrencyRates(modelBuilder);
	}

	/// <summary>
	/// Seed کردن سازمان‌ها
	/// </summary>
	private static void SeedOrganizations(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Organization>().HasData(
			new Organization
			{
				Id = DefaultOrganizationId,
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
				Id = TestOrganizationId,
				Name = "شرکت آزمایشی",
				Code = "TEST001",
				IsActive = true,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			}
		);
	}

	/// <summary>
	/// Seed کردن گروه‌های دسترسی
	/// </summary>
	private static void SeedPermissionGroups(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PermissionGroup>().HasData(
			new PermissionGroup
			{
				Id = UserManagementGroupId,
				Name = "مدیریت کاربران",
				Description = "دسترسی‌های مربوط به مدیریت کاربران سیستم",
				IsSystemGroup = true,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new PermissionGroup
			{
				Id = RoleManagementGroupId,
				Name = "مدیریت نقش‌ها",
				Description = "دسترسی‌های مربوط به مدیریت نقش‌ها و سطوح دسترسی",
				IsSystemGroup = true,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new PermissionGroup
			{
				Id = OrganizationManagementGroupId,
				Name = "مدیریت سازمان",
				Description = "دسترسی‌های مربوط به تنظیمات و مدیریت سازمان",
				IsSystemGroup = true,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new PermissionGroup
			{
				Id = FinancialManagementGroupId,
				Name = "مدیریت مالی",
				Description = "دسترسی‌های مربوط به عملیات مالی و تسهیلات",
				IsSystemGroup = false,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new PermissionGroup
			{
				Id = ReportManagementGroupId,
				Name = "گزارشات",
				Description = "دسترسی‌های مربوط به مشاهده و تولید گزارشات",
				IsSystemGroup = false,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			}
		);
	}

	/// <summary>
	/// Seed کردن دسترسی‌ها
	/// </summary>
	private static void SeedPermissions(ModelBuilder modelBuilder)
	{
		var permissions = new List<Permission>();

		// دسترسی‌های مدیریت کاربران
		permissions.AddRange(new[]
		{
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = UserManagementGroupId, Name = "مشاهده کاربران", Code = "Users.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = UserManagementGroupId, Name = "ایجاد کاربر", Code = "Users.Create", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = UserManagementGroupId, Name = "ویرایش کاربر", Code = "Users.Update", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = UserManagementGroupId, Name = "حذف کاربر", Code = "Users.Delete", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = UserManagementGroupId, Name = "تغییر رمز عبور دیگران", Code = "Users.ChangeOthersPassword", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
	});

		// دسترسی‌های مدیریت نقش‌ها
		permissions.AddRange(new[]
		{
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = RoleManagementGroupId, Name = "مشاهده نقش‌ها", Code = "Roles.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = RoleManagementGroupId, Name = "ایجاد نقش", Code = "Roles.Create", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = RoleManagementGroupId, Name = "ویرایش نقش", Code = "Roles.Update", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = RoleManagementGroupId, Name = "حذف نقش", Code = "Roles.Delete", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = RoleManagementGroupId, Name = "مدیریت دسترسی‌ها", Code = "Permissions.Manage", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
	});

		// دسترسی‌های مدیریت سازمان
		permissions.AddRange(new[]
		{
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = OrganizationManagementGroupId, Name = "مشاهده اطلاعات سازمان", Code = "Organization.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = OrganizationManagementGroupId, Name = "ویرایش اطلاعات سازمان", Code = "Organization.Update", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = OrganizationManagementGroupId, Name = "مدیریت تنظیمات", Code = "Organization.ManageSettings", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
	});

		// دسترسی‌های مالی
		permissions.AddRange(new[]
		{
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = FinancialManagementGroupId, Name = "مشاهده تسهیلات", Code = "Loans.View", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = FinancialManagementGroupId, Name = "مدیریت تسهیلات", Code = "Loans.Manage", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = FinancialManagementGroupId, Name = "تایید تسهیلات", Code = "Loans.Approve", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
	});

		// دسترسی‌های گزارشات
		permissions.AddRange(new[]
		{
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = ReportManagementGroupId, Name = "مشاهده گزارشات مالی", Code = "Financial.ViewReports", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
		new Permission { Id = Guid.NewGuid(), PermissionGroupId = ReportManagementGroupId, Name = "دانلود گزارشات", Code = "Reports.Download", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
	});

		modelBuilder.Entity<Permission>().HasData(permissions);
	}

	/// <summary>
	/// Seed کردن نقش‌ها
	/// </summary>
	private static void SeedRoles(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Role>().HasData(
			new Role
			{
				Id = SystemAdminRoleId,
				OrganizationId = DefaultOrganizationId,
				Name = "مدیر سیستم",
				Description = "دسترسی کامل به تمام بخش‌های سیستم",
				IsSystemRole = true,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new Role
			{
				Id = OrganizationAdminRoleId,
				OrganizationId = DefaultOrganizationId,
				Name = "مدیر سازمان",
				Description = "مدیریت کامل سازمان و کاربران",
				IsSystemRole = true,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new Role
			{
				Id = UserRoleId,
				OrganizationId = DefaultOrganizationId,
				Name = "کاربر عادی",
				Description = "دسترسی‌های پایه کاربری",
				IsSystemRole = false,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new Role
			{
				Id = AuditorRoleId,
				OrganizationId = DefaultOrganizationId,
				Name = "حسابرس",
				Description = "دسترسی مشاهده گزارشات و بررسی عملیات",
				IsSystemRole = false,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			}
		);
	}

	/// <summary>
	/// Seed کردن کاربران
	/// </summary>
	private static void SeedUsers(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().HasData(
			new User
			{
				Id = SystemAdminUserId,
				OrganizationId = DefaultOrganizationId,
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
				Id = TestAdminUserId,
				OrganizationId = TestOrganizationId,
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
				Id = TestUserId,
				OrganizationId = DefaultOrganizationId,
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
		);
	}

	/// <summary>
	/// Seed کردن ارتباط کاربران و نقش‌ها
	/// </summary>
	private static void SeedUserRoles(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserRole>().HasData(
			new UserRole
			{
				UserId = SystemAdminUserId,
				RoleId = SystemAdminRoleId,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new UserRole
			{
				UserId = TestAdminUserId,
				RoleId = OrganizationAdminRoleId,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			new UserRole
			{
				UserId = TestUserId,
				RoleId = UserRoleId,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			}
		);
	}

	/// <summary>
	/// Seed کردن ارتباط نقش‌ها و گروه‌های دسترسی
	/// </summary>
	private static void SeedRolePermissionGroups(ModelBuilder modelBuilder)
	{
		// مدیر سیستم - دسترسی کامل
		modelBuilder.Entity<RolePermissionGroup>().HasData(
			new RolePermissionGroup { RoleId = SystemAdminRoleId, PermissionGroupId = UserManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
			new RolePermissionGroup { RoleId = SystemAdminRoleId, PermissionGroupId = RoleManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
			new RolePermissionGroup { RoleId = SystemAdminRoleId, PermissionGroupId = OrganizationManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
			new RolePermissionGroup { RoleId = SystemAdminRoleId, PermissionGroupId = FinancialManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
			new RolePermissionGroup { RoleId = SystemAdminRoleId, PermissionGroupId = ReportManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
		);

		// مدیر سازمان
		modelBuilder.Entity<RolePermissionGroup>().HasData(
			new RolePermissionGroup { RoleId = OrganizationAdminRoleId, PermissionGroupId = UserManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
			new RolePermissionGroup { RoleId = OrganizationAdminRoleId, PermissionGroupId = OrganizationManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
			new RolePermissionGroup { RoleId = OrganizationAdminRoleId, PermissionGroupId = ReportManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
		);

		// حسابرس
		modelBuilder.Entity<RolePermissionGroup>().HasData(
			new RolePermissionGroup { RoleId = AuditorRoleId, PermissionGroupId = ReportManagementGroupId, CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
		);
	}

	/// <summary>
	/// Seed کردن ارزها
	/// </summary>
	private static void SeedCurrencies(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Currency>().HasData(
			new Currency
			{
				Id = IrrCurrencyId,
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
				Id = UsdCurrencyId,
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
				Id = EurCurrencyId,
				Code = "EUR",
				Name = "یورو",
				Symbol = "€",
				DecimalPlaces = 2,
				IsActive = true,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			}
		);
	}

	/// <summary>
	/// Seed کردن نرخ ارزها
	/// </summary>
	private static void SeedCurrencyRates(ModelBuilder modelBuilder)
	{
		var effectiveDate = DateTime.UtcNow.Date;

		modelBuilder.Entity<CurrencyRate>().HasData(
			// نرخ دلار به ریال
			new CurrencyRate
			{
				Id = Guid.NewGuid(),
				FromCurrencyId = UsdCurrencyId,
				ToCurrencyId = IrrCurrencyId,
				Rate = 42000,
				EffectiveDate = effectiveDate,
				ExpiryDate = null,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			// نرخ یورو به ریال
			new CurrencyRate
			{
				Id = Guid.NewGuid(),
				FromCurrencyId = EurCurrencyId,
				ToCurrencyId = IrrCurrencyId,
				Rate = 45500,
				EffectiveDate = effectiveDate,
				ExpiryDate = null,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			},
			// نرخ دلار به یورو
			new CurrencyRate
			{
				Id = Guid.NewGuid(),
				FromCurrencyId = UsdCurrencyId,
				ToCurrencyId = EurCurrencyId,
				Rate = 0.92m,
				EffectiveDate = effectiveDate,
				ExpiryDate = null,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = "System"
			}
		);
	}
}