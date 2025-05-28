using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Enums;
using LendTech.SharedKernel.Extensions;

namespace LendTech.Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository لاگ‌های ممیزی
/// </summary>
public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> GetByUserAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(a => a.UserId == userId);

        if (fromDate.HasValue)
            query = query.Where(a => a.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.CreatedAt <= toDate.Value);

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> GetByOrganizationAsync(Guid organizationId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(a => a.OrganizationId == organizationId);

        if (fromDate.HasValue)
            query = query.Where(a => a.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.CreatedAt <= toDate.Value);

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> GetByActionAsync(string action, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(a => a.Action == action);

        if (fromDate.HasValue)
            query = query.Where(a => a.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.CreatedAt <= toDate.Value);

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await AddAsync(auditLog, cancellationToken);
    }

    /// <inheritdoc />
    public async Task LogAsync(
        Guid organizationId,
        string entityName,
        string entityId,
        AuditAction action,
        Guid? userId = null,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            OrganizationId = organizationId,
            UserId = userId,
            EntityName = entityName,
            EntityId = entityId,
            Action = action.ToString(),
            OldValues = oldValues?.ToJson(),
            NewValues = newValues?.ToJson(),
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        await LogAsync(auditLog, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CleanupOldLogsAsync(int daysToKeep, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        
        var logsToDelete = await _dbSet
            .Where(a => a.CreatedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(logsToDelete);
        await _context.SaveChangesAsync(cancellationToken);

        return logsToDelete.Count;
    }

    /// <inheritdoc />
    public async Task<AuditStatistics> GetStatisticsAsync(Guid organizationId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var stats = new AuditStatistics();

        var logs = await _dbSet
            .Where(a => a.OrganizationId == organizationId && 
                       a.CreatedAt >= fromDate && 
                       a.CreatedAt <= toDate)
            .ToListAsync(cancellationToken);

        // آمار بر اساس نوع عملیات
        stats.ActionCounts = logs
            .GroupBy(a => a.Action)
            .ToDictionary(g => g.Key, g => g.Count());

        // آمار بر اساس موجودیت
        stats.EntityCounts = logs
            .GroupBy(a => a.EntityName)
            .ToDictionary(g => g.Key, g => g.Count());

        // آمار فعالیت کاربران
        stats.UserActivityCounts = logs
            .Where(a => a.UserId.HasValue)
            .GroupBy(a => a.UserId!.Value.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // آمار روزانه
        stats.DailyActivityCounts = logs
            .GroupBy(a => a.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => (g.Key, g.Count()))
            .ToList();

        return stats;
    }
}
