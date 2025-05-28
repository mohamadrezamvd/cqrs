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
/// پیاده‌سازی Repository دسترسی‌ها
/// </summary>
public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Permission>> GetByGroupIdAsync(Guid permissionGroupId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.PermissionGroupId == permissionGroupId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Permission>> GetAllWithGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.PermissionGroup)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsCodeExistsAsync(string code, Guid? excludePermissionId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.Code == code);
        
        if (excludePermissionId.HasValue)
        {
            query = query.Where(p => p.Id != excludePermissionId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Permission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissionGroups)
            .SelectMany(rpg => rpg.PermissionGroup.Permissions)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RolePermissionGroup>()
            .Where(rpg => rpg.RoleId == roleId)
            .SelectMany(rpg => rpg.PermissionGroup.Permissions)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
