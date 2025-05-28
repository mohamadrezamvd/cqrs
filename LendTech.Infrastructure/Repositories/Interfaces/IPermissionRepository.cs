using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository دسترسی‌ها
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>
    /// دریافت دسترسی با کد
    /// </summary>
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت دسترسی‌های یک گروه
    /// </summary>
    Task<List<Permission>> GetByGroupIdAsync(Guid permissionGroupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تمام دسترسی‌ها به همراه گروه
    /// </summary>
    Task<List<Permission>> GetAllWithGroupsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود کد دسترسی
    /// </summary>
    Task<bool> IsCodeExistsAsync(string code, Guid? excludePermissionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت دسترسی‌های کاربر
    /// </summary>
    Task<List<Permission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت دسترسی‌های نقش
    /// </summary>
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
}
