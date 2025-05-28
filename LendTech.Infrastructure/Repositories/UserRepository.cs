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
/// پیاده‌سازی Repository کاربران
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<User?> GetByUsernameAsync(string username, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username && u.OrganizationId == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email && u.OrganizationId == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByMobileAsync(string mobileNumber, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber && u.OrganizationId == organizationId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetWithPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissionGroups)
                        .ThenInclude(rpg => rpg.PermissionGroup)
                            .ThenInclude(pg => pg.Permissions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsUsernameExistsAsync(string username, Guid organizationId, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Username == username && u.OrganizationId == organizationId);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsEmailExistsAsync(string email, Guid organizationId, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Email == email && u.OrganizationId == organizationId);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task IncrementFailedAccessCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.AccessFailedCount++;
            await UpdateAsync(user, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task ResetFailedAccessCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.AccessFailedCount = 0;
            await UpdateAsync(user, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task LockUserAsync(Guid userId, DateTime lockoutEnd, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.IsLocked = true;
            user.LockoutEnd = lockoutEnd;
            await UpdateAsync(user, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task UnlockUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.IsLocked = false;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            await UpdateAsync(user, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.PasswordHash = newPasswordHash;
            await UpdateAsync(user, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetWithPermissionsAsync(userId, cancellationToken);
        
        if (user == null)
            return new List<string>();

        return user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissionGroups)
            .SelectMany(rpg => rpg.PermissionGroup.Permissions)
            .Select(p => p.Code)
            .Distinct()
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetWithRolesAsync(userId, cancellationToken);
        
        if (user == null)
            return new List<Role>();

        return user.UserRoles.Select(ur => ur.Role).ToList();
    }
}
