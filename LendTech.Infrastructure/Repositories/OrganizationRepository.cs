using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Models;
using LendTech.SharedKernel.Extensions;

namespace LendTech.Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository سازمان‌ها
/// </summary>
public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Organization?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(o => o.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsCodeExistsAsync(string code, Guid? excludeOrganizationId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(o => o.Code == code);
        
        if (excludeOrganizationId.HasValue)
        {
            query = query.Where(o => o.Id != excludeOrganizationId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OrganizationSettings?> GetSettingsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var organization = await GetByIdAsync(organizationId, cancellationToken);
        
        if (organization?.Settings == null)
            return new OrganizationSettings();

        return organization.Settings.FromJson<OrganizationSettings>();
    }

    /// <inheritdoc />
    public async Task SaveSettingsAsync(Guid organizationId, OrganizationSettings settings, CancellationToken cancellationToken = default)
    {
        var organization = await GetByIdAsync(organizationId, cancellationToken);
        
        if (organization != null)
        {
            organization.Settings = settings.ToJson();
            await UpdateAsync(organization, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<List<FeatureFlag>> GetFeaturesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var organization = await GetByIdAsync(organizationId, cancellationToken);
        
        if (organization?.Features == null)
            return new List<FeatureFlag>();

        return organization.Features.FromJson<List<FeatureFlag>>() ?? new List<FeatureFlag>();
    }

    /// <inheritdoc />
    public async Task<bool> IsFeatureEnabledAsync(Guid organizationId, string featureName, CancellationToken cancellationToken = default)
    {
        var features = await GetFeaturesAsync(organizationId, cancellationToken);
        var feature = features.FirstOrDefault(f => f.Name == featureName);
        
        return feature?.IsActiveAt(DateTime.UtcNow) ?? false;
    }

    /// <inheritdoc />
    public async Task ToggleActiveStatusAsync(Guid organizationId, bool isActive, CancellationToken cancellationToken = default)
    {
        var organization = await GetByIdAsync(organizationId, cancellationToken);
        
        if (organization != null)
        {
            organization.IsActive = isActive;
            await UpdateAsync(organization, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<OrganizationStatistics> GetStatisticsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var stats = new OrganizationStatistics();

        // آمار کاربران
        stats.TotalUsers = await _context.Users
            .CountAsync(u => u.OrganizationId == organizationId, cancellationToken);
        
        stats.ActiveUsers = await _context.Users
            .CountAsync(u => u.OrganizationId == organizationId && u.IsActive && !u.IsLocked, cancellationToken);

        // آمار نقش‌ها
        stats.TotalRoles = await _context.Roles
            .CountAsync(r => r.OrganizationId == organizationId, cancellationToken);

        // آخرین فعالیت
        stats.LastActivityDate = await _context.AuditLogs
            .Where(a => a.OrganizationId == organizationId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => a.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        // آمار تراکنش‌ها (فرضی - باید با جداول واقعی جایگزین شود)
        stats.TotalTransactions = 0;
        stats.TotalTransactionAmount = 0;

        return stats;
    }
}
