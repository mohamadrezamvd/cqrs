using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.Infrastructure.Security;

namespace LendTech.Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository مدیریت توکن‌های کاربر
/// </summary>
public class UserTokenRepository : Repository<UserToken>, IUserTokenRepository
{
    public UserTokenRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UserToken> SaveTokenAsync(UserToken token, CancellationToken cancellationToken = default)
    {
        // هش کردن توکن‌ها قبل از ذخیره
        token.AccessToken = TokenHasher.HashToken(token.AccessToken);
        token.RefreshToken = TokenHasher.HashToken(token.RefreshToken);
        token.CreatedAt = DateTime.UtcNow;
        
        await _dbSet.AddAsync(token, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return token;
    }

    /// <inheritdoc />
    public async Task<UserToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserToken?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var hashedToken = TokenHasher.HashToken(accessToken);
        return await _dbSet
            .FirstOrDefaultAsync(t => t.AccessToken == hashedToken && !t.IsRevoked, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var hashedToken = TokenHasher.HashToken(refreshToken);
        return await _dbSet
            .FirstOrDefaultAsync(t => t.RefreshToken == hashedToken && !t.IsRevoked, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<UserToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.UserId == userId && !t.IsRevoked && t.RefreshTokenExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RevokeTokenAsync(Guid tokenId, string? revokedBy = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        var token = await GetByIdAsync(tokenId, cancellationToken);
        if (token != null)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedBy = revokedBy;
            token.RevokedReason = reason;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task RevokeAllUserTokensAsync(Guid userId, string? revokedBy = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbSet
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedBy = revokedBy;
            token.RevokedReason = reason;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _dbSet
            .Where(t => t.RefreshTokenExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync(cancellationToken);
    }
} 