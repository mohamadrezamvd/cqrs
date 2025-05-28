using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository گروه‌های دسترسی
/// </summary>
public interface IPermissionGroupRepository : IRepository<PermissionGroup>
{
    /// <summary>
    /// دریافت گروه با نام
    /// </summary>
    Task<PermissionGroup?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت گروه به همراه دسترسی‌ها
    /// </summary>
    Task<PermissionGroup?> GetWithPermissionsAsync(Guid groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت گروه‌های سیستمی
    /// </summary>
    Task<List<PermissionGroup>> GetSystemGroupsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود نام گروه
    /// </summary>
    Task<bool> IsNameExistsAsync(string name, Guid? excludeGroupId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نقش‌های گروه
    /// </summary>
    Task<List<Role>> GetGroupRolesAsync(Guid groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// شمارش نقش‌های گروه
    /// </summary>
    Task<int> CountGroupRolesAsync(Guid groupId, CancellationToken cancellationToken = default);
}
