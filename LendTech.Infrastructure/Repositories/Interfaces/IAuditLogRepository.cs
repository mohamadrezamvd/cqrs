using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;
using LendTech.SharedKernel.Enums;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository لاگ‌های ممیزی
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    /// <summary>
    /// دریافت لاگ‌های یک موجودیت
    /// </summary>
    Task<List<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لاگ‌های یک کاربر
    /// </summary>
    Task<List<AuditLog>> GetByUserAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لاگ‌های یک سازمان
    /// </summary>
    Task<List<AuditLog>> GetByOrganizationAsync(Guid organizationId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لاگ‌های یک عملیات خاص
    /// </summary>
    Task<List<AuditLog>> GetByActionAsync(string action, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// ثبت لاگ جدید
    /// </summary>
    Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// ثبت لاگ با اطلاعات
    /// </summary>
    Task LogAsync(
        Guid organizationId,
        string entityName,
        string entityId,
        AuditAction action,
        Guid? userId = null,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// پاکسازی لاگ‌های قدیمی
    /// </summary>
    Task<int> CleanupOldLogsAsync(int daysToKeep, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت آمار لاگ‌ها
    /// </summary>
    Task<AuditStatistics> GetStatisticsAsync(Guid organizationId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}

/// <summary>
/// مدل آمار Audit
/// </summary>
public class AuditStatistics
{
    public Dictionary<string, int> ActionCounts { get; set; } = new();
    public Dictionary<string, int> EntityCounts { get; set; } = new();
    public Dictionary<string, int> UserActivityCounts { get; set; } = new();
    public List<(DateTime Date, int Count)> DailyActivityCounts { get; set; } = new();
}
