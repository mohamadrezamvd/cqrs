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
/// پیاده‌سازی Repository گروه‌های دسترسی
/// </summary>
public class PermissionGroupRepository : Repository<PermissionGroup>, IPermissionGroupRepository
{
    public PermissionGroupRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<PermissionGroup?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pg => pg.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PermissionGroup?> GetWithPermissionsAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(pg => pg.Permissions)
            .FirstOrDefaultAsync(pg => pg.Id == groupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<PermissionGroup>> GetSystemGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pg => pg.IsSystemGroup)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsNameExistsAsync(string name, Guid? excludeGroupId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(pg => pg.Name == name);
        
        if (excludeGroupId.HasValue)
        {
            query = query.Where(pg => pg.Id != excludeGroupId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Role>> GetGroupRolesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RolePermissionGroup>()
            .Where(rpg => rpg.PermissionGroupId == groupId)
            .Select(rpg => rpg.Role)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountGroupRolesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RolePermissionGroup>()
            .CountAsync(rpg => rpg.PermissionGroupId == groupId, cancellationToken);
    }
}
