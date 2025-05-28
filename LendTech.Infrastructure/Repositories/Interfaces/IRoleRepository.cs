using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository نقش‌ها
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// دریافت نقش با نام
    /// </summary>
    Task<Role?> GetByNameAsync(string name, Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نقش به همراه گروه‌های دسترسی
    /// </summary>
    Task<Role?> GetWithPermissionGroupsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نقش‌های سیستمی
    /// </summary>
    Task<List<Role>> GetSystemRolesAsync(Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود نام نقش
    /// </summary>
    Task<bool> IsNameExistsAsync(string name, Guid organizationId, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// اختصاص گروه‌های دسترسی به نقش
    /// </summary>
    Task AssignPermissionGroupsAsync(Guid roleId, List<Guid> permissionGroupIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف گروه‌های دسترسی از نقش
    /// </summary>
    Task RemovePermissionGroupsAsync(Guid roleId, List<Guid> permissionGroupIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت دسترسی‌های نقش
    /// </summary>
    Task<List<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کاربران نقش
    /// </summary>
    Task<List<User>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// شمارش کاربران نقش
    /// </summary>
    Task<int> CountRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default);
}
