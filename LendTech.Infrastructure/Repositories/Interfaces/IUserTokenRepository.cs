using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository مدیریت توکن‌های کاربر
/// </summary>
public interface IUserTokenRepository
{
    /// <summary>
    /// ذخیره توکن جدید
    /// </summary>
    Task<UserToken> SaveTokenAsync(UserToken token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت توکن با شناسه
    /// </summary>
    Task<UserToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت توکن با Access Token
    /// </summary>
    Task<UserToken?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت توکن با Refresh Token
    /// </summary>
    Task<UserToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// دریافت توکن‌های فعال کاربر
    /// </summary>
    Task<List<UserToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ابطال توکن
    /// </summary>
    Task RevokeTokenAsync(Guid tokenId, string? revokedBy = null, string? reason = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// ابطال همه توکن‌های کاربر
    /// </summary>
    Task RevokeAllUserTokensAsync(Guid userId, string? revokedBy = null, string? reason = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// پاکسازی توکن‌های منقضی شده
    /// </summary>
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
} 