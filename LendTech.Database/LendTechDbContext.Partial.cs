using LendTech.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LendTech.Database
{
	/// <summary>
	/// بخش Partial از DbContext برای مدیریت Auditing و Filtering
	/// این فایل با Scaffold تداخل ندارد
	/// </summary>
	public partial class LendTechDbContext
	{
		// سرویس‌های مورد نیاز برای Auditing
		private readonly string? _currentUserId;
		private readonly Guid? _currentOrganizationId;
		/// <summary>
		/// کانستراکتور برای تزریق اطلاعات کاربر جاری
		/// </summary>
		public LendTechDbContext(
			DbContextOptions<LendTechDbContext> options,
			string? currentUserId,
			Guid? currentOrganizationId)
			: base(options)
		{
			_currentUserId = currentUserId;
			_currentOrganizationId = currentOrganizationId;
		}

		/// <summary>
		/// اعمال فیلترهای Global Query
		/// </summary>
		partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
		{
			// فقط اعمال فیلترها - بدون Configuration
			ApplyGlobalQueryFilters(modelBuilder);
		}

		/// <summary>
		/// بازنویسی SaveChanges برای پیاده‌سازی Audit Trail
		/// </summary>
		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await HandleAuditing();
			return await base.SaveChangesAsync(cancellationToken);
		}

		/// <summary>
		/// بازنویسی SaveChanges برای پیاده‌سازی Audit Trail
		/// </summary>
		public override int SaveChanges()
		{
			HandleAuditing().GetAwaiter().GetResult();
			return base.SaveChanges();
		}

		/// <summary>
		/// اعمال فیلترهای Global Query برای Multi-Tenancy
		/// </summary>
		private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
		{
			if (_currentOrganizationId.HasValue)
			{
				// فیلتر برای Users
				modelBuilder.Entity<User>().HasQueryFilter(u =>
					u.OrganizationId == _currentOrganizationId.Value && !u.IsDeleted);

				// فیلتر برای Roles
				modelBuilder.Entity<Role>().HasQueryFilter(r =>
					r.OrganizationId == _currentOrganizationId.Value && !r.IsDeleted);

				// فیلتر برای OutboxEvents
				modelBuilder.Entity<OutboxEvent>().HasQueryFilter(o =>
					o.OrganizationId == _currentOrganizationId.Value);

				// فیلتر برای InboxEvents
				modelBuilder.Entity<InboxEvent>().HasQueryFilter(i =>
					i.OrganizationId == _currentOrganizationId.Value);

				// فیلتر برای AuditLogs
				modelBuilder.Entity<AuditLog>().HasQueryFilter(a =>
					a.OrganizationId == _currentOrganizationId.Value);
			}
			else
			{
				// فقط فیلتر Soft Delete برای موجودیت‌های SoftDeletable
				modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
				modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);
				modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted);
				modelBuilder.Entity<Permission>().HasQueryFilter(p => !p.IsDeleted);
				modelBuilder.Entity<PermissionGroup>().HasQueryFilter(pg => !pg.IsDeleted);
			}
		}

		/// <summary>
		/// مدیریت فیلدهای ممیزی
		/// </summary>
		private async Task HandleAuditing()
		{
			var entries = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Added ||
						   e.State == EntityState.Modified ||
						   e.State == EntityState.Deleted);

			var currentTime = DateTime.UtcNow;
			var auditLogs = new List<AuditLog>();

			foreach (var entry in entries)
			{
				// برای موجودیت‌های BaseEntity
				if (entry.Entity is BaseEntity baseEntity)
				{
					switch (entry.State)
					{
						case EntityState.Added:
							baseEntity.CreatedAt = currentTime;
							baseEntity.CreatedBy = _currentUserId;
							break;

						case EntityState.Modified:
							baseEntity.ModifiedAt = currentTime;
							baseEntity.ModifiedBy = _currentUserId;
							// جلوگیری از تغییر فیلدهای Created
							entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
							entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
							break;
					}
				}

				// برای موجودیت‌های SoftDeletable
				if (entry.Entity is SoftDeletableEntity softDeletableEntity && entry.State == EntityState.Deleted)
				{
					// تبدیل حذف فیزیکی به حذف منطقی
					entry.State = EntityState.Modified;
					softDeletableEntity.IsDeleted = true;
					softDeletableEntity.DeletedAt = currentTime;
					softDeletableEntity.DeletedBy = _currentUserId;
				}

				// ایجاد Audit Log برای تغییرات مهم
				if (_currentOrganizationId.HasValue && ShouldAudit(entry))
				{
					var auditLog = CreateAuditLog(entry);
					if (auditLog != null)
						auditLogs.Add(auditLog);
				}
			}

			// اضافه کردن Audit Logs
			if (auditLogs.Any())
			{
				await AuditLogs.AddRangeAsync(auditLogs);
			}
		}

		/// <summary>
		/// تعیین اینکه آیا این تغییر باید لاگ شود یا نه
		/// </summary>
		private bool ShouldAudit(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
		{
			// موجودیت‌هایی که نیاز به Audit دارند
			var auditableTypes = new[]
			{
			typeof(User),
			typeof(Role),
			typeof(Permission),
			typeof(PermissionGroup),
			typeof(UserRole),
			typeof(RolePermissionGroup)
		};

			return auditableTypes.Contains(entry.Entity.GetType());
		}

		/// <summary>
		/// ایجاد لاگ ممیزی
		/// </summary>
		private AuditLog? CreateAuditLog(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
		{
			var auditLog = new AuditLog
			{
				Id = Guid.NewGuid(),
				OrganizationId = _currentOrganizationId!.Value,
				UserId = string.IsNullOrEmpty(_currentUserId) ? null : Guid.Parse(_currentUserId),
				EntityName = entry.Entity.GetType().Name,
				Action = entry.State.ToString(),
				CreatedAt = DateTime.UtcNow
			};

			// تنظیم EntityId
			if (entry.Entity is BaseEntity entity)
			{
				auditLog.EntityId = entity.Id.ToString();
			}

			// ذخیره مقادیر قدیم و جدید به صورت JSON
			var oldValues = new Dictionary<string, object?>();
			var newValues = new Dictionary<string, object?>();

			foreach (var property in entry.Properties)
			{
				var propertyName = property.Metadata.Name;

				// عدم لاگ کردن فیلدهای حساس
				if (propertyName == nameof(User.PasswordHash))
					continue;

				switch (entry.State)
				{
					case EntityState.Added:
						newValues[propertyName] = property.CurrentValue;
						break;

					case EntityState.Deleted:
						oldValues[propertyName] = property.OriginalValue;
						break;

					case EntityState.Modified:
						if (property.IsModified)
						{
							oldValues[propertyName] = property.OriginalValue;
							newValues[propertyName] = property.CurrentValue;
						}
						break;
				}
			}

			if (oldValues.Any())
				auditLog.OldValues = System.Text.Json.JsonSerializer.Serialize(oldValues);

			if (newValues.Any())
				auditLog.NewValues = System.Text.Json.JsonSerializer.Serialize(newValues);

			return auditLog;
		}
	}
}
