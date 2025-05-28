using System;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;
using LendTech.SharedKernel.Models;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository سازمان‌ها
/// </summary>
public interface IOrganizationRepository : IRepository<Organization>
{
    /// <summary>
    /// دریافت سازمان با کد
    /// </summary>
    Task<Organization?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود کد سازمان
    /// </summary>
    Task<bool> IsCodeExistsAsync(string code, Guid? excludeOrganizationId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تنظیمات سازمان
    /// </summary>
    Task<OrganizationSettings?> GetSettingsAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره تنظیمات سازمان
    /// </summary>
    Task SaveSettingsAsync(Guid organizationId, OrganizationSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت Feature Flags سازمان
    /// </summary>
    Task<List<FeatureFlag>> GetFeaturesAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی فعال بودن Feature
    /// </summary>
    Task<bool> IsFeatureEnabledAsync(Guid organizationId, string featureName, CancellationToken cancellationToken = default);

    /// <summary>
    /// فعال/غیرفعال کردن سازمان
    /// </summary>
    Task ToggleActiveStatusAsync(Guid organizationId, bool isActive, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت آمار سازمان
    /// </summary>
    Task<OrganizationStatistics> GetStatisticsAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

/// <summary>
/// مدل آمار سازمان
/// </summary>
public class OrganizationStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalRoles { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public long TotalTransactions { get; set; }
    public decimal TotalTransactionAmount { get; set; }
}
