using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;

namespace LendTech.Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository نقش‌ها
/// </summary>
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Role?> GetByNameAsync(string name, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Name == name && r.OrganizationId == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Role?> GetWithPermissionGroupsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.RolePermissionGroups)
                .ThenInclude(rpg => rpg.PermissionGroup)
                    .ThenInclude(pg => pg.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Role>> GetSystemRolesAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.OrganizationId == organizationId && r.IsSystemRole)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsNameExistsAsync(string name, Guid organizationId, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(r => r.Name == name && r.OrganizationId == organizationId);
        
        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRoleId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AssignPermissionGroupsAsync(Guid roleId, List<Guid> permissionGroupIds, CancellationToken cancellationToken = default)
    {
        var role = await GetWithPermissionGroupsAsync(roleId, cancellationToken);
        if (role == null) return;

        // حذف گروه‌های قبلی
        _context.Set<RolePermissionGroup>().RemoveRange(role.RolePermissionGroups);

        // اضافه کردن گروه‌های جدید
        foreach (var groupId in permissionGroupIds)
        {
            _context.Set<RolePermissionGroup>().Add(new RolePermissionGroup
            {
                RoleId = roleId,
                PermissionGroupId = groupId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemovePermissionGroupsAsync(Guid roleId, List<Guid> permissionGroupIds, CancellationToken cancellationToken = default)
    {
        var groupsToRemove = await _context.Set<RolePermissionGroup>()
            .Where(rpg => rpg.RoleId == roleId && permissionGroupIds.Contains(rpg.PermissionGroupId))
            .ToListAsync(cancellationToken);

        _context.Set<RolePermissionGroup>().RemoveRange(groupsToRemove);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await GetWithPermissionGroupsAsync(roleId, cancellationToken);
        
        if (role == null)
            return new List<string>();

        return role.RolePermissionGroups
            .SelectMany(rpg => rpg.PermissionGroup.Permissions)
            .Select(p => p.Code)
            .Distinct()
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<User>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.User)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .CountAsync(ur => ur.RoleId == roleId, cancellationToken);
    }
}
