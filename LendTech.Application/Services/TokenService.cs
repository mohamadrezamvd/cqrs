using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LendTech.Application.DTOs.Auth;
using LendTech.Application.Services.Interfaces;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Redis.Interfaces;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.Infrastructure.Security;
using LendTech.SharedKernel.Constants;
namespace LendTech.Application.Services;
/// <summary>
/// سرویس مدیریت توکن
/// </summary>
public class TokenService : ITokenService
{
private readonly IConfiguration _configuration;
private readonly ICacheService _cacheService;
private readonly IUserTokenRepository _userTokenRepository;
private readonly ILogger<TokenService> _logger;
public TokenService(
    IConfiguration configuration,
    ICacheService cacheService,
    IUserTokenRepository userTokenRepository,
    ILogger<TokenService> logger)
{
    _configuration = configuration;
    _cacheService = cacheService;
    _userTokenRepository = userTokenRepository;
    _logger = logger;
}

/// <inheritdoc />
public async Task<string> GenerateAccessTokenAsync(Guid userId, Guid organizationId, List<string> roles, List<string> permissions)
{
    try
    {
        var claims = new Dictionary<string, string>
        {
            [SecurityConstants.Claims.UserId] = userId.ToString(),
            [SecurityConstants.Claims.OrganizationId] = organizationId.ToString()
        };

        // اضافه کردن نقش‌ها و دسترسی‌ها به صورت JSON
        if (roles.Any())
        {
            claims[SecurityConstants.Claims.Roles] = string.Join(",", roles);
        }

        if (permissions.Any())
        {
            claims[SecurityConstants.Claims.Permissions] = string.Join(",", permissions);
        }

        var accessToken = JwtHelper.GenerateToken(
            claims,
            _configuration["Jwt:SecretKey"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!,
            int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));

        var refreshToken = JwtHelper.GenerateRefreshToken();

        // ذخیره توکن در دیتابیس
        var userToken = new UserToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(SystemConstants.RefreshTokenExpirationDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _userTokenRepository.SaveTokenAsync(userToken);

        _logger.LogInformation("توکن دسترسی برای کاربر {UserId} تولید شد", userId);

        return accessToken;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در تولید توکن دسترسی");
        throw;
    }
}

/// <inheritdoc />
public async Task<string> GenerateRefreshTokenAsync()
{
    await Task.CompletedTask;
    return JwtHelper.GenerateRefreshToken();
}

/// <inheritdoc />
public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
{
    try
    {
        var principal = JwtHelper.ValidateToken(
            token,
            _configuration["Jwt:SecretKey"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!);

        if (principal == null)
            return null;

        // بررسی وضعیت توکن در دیتابیس
        var userToken = await _userTokenRepository.GetByAccessTokenAsync(token);
        if (userToken == null || userToken.IsRevoked || userToken.AccessTokenExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("توکن نامعتبر یا باطل شده است");
            return null;
        }

        return principal;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در اعتبارسنجی توکن");
        return null;
    }
}

/// <inheritdoc />
public async Task<UserInfoDto?> GetUserInfoFromTokenAsync(string token)
{
    var principal = await ValidateTokenAsync(token);
    
    if (principal == null)
        return null;

    var userId = JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.UserId);
    
    if (string.IsNullOrEmpty(userId))
        return null;

    var rolesStr = JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.Roles);
    var permissionsStr = JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.Permissions);

    return new UserInfoDto
    {
        Id = Guid.Parse(userId),
        Username = "", // باید از دیتابیس بخوانیم
        Email = "",
        FullName = "",
        Roles = rolesStr?.Split(',').ToList() ?? new List<string>(),
        Permissions = permissionsStr?.Split(',').ToList() ?? new List<string>()
    };
}

/// <inheritdoc />
public async Task RevokeTokenAsync(string token)
{
    try
    {
        var userToken = await _userTokenRepository.GetByAccessTokenAsync(token);
        if (userToken != null)
        {
            await _userTokenRepository.RevokeTokenAsync(userToken.Id, "System", "Logout");
            _logger.LogInformation("توکن کاربر {UserId} ابطال شد", userToken.UserId);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ابطال توکن");
    }
}

/// <inheritdoc />
public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
{
    try
    {
        var userToken = await _userTokenRepository.GetByRefreshTokenAsync(refreshToken);
        if (userToken == null || userToken.IsRevoked || userToken.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh Token نامعتبر یا منقضی شده است");
            return null;
        }

        // ابطال توکن قبلی
        await _userTokenRepository.RevokeTokenAsync(userToken.Id, "System", "Token Refresh");

        // تولید توکن جدید
        var principal = JwtHelper.ValidateToken(
            userToken.AccessToken,
            _configuration["Jwt:SecretKey"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!);

        if (principal == null)
            return null;

        var userId = Guid.Parse(JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.UserId)!);
        var organizationId = Guid.Parse(JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.OrganizationId)!);
        var roles = JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.Roles)?.Split(',').ToList() ?? new List<string>();
        var permissions = JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.Permissions)?.Split(',').ToList() ?? new List<string>();

        var newAccessToken = await GenerateAccessTokenAsync(userId, organizationId, roles, permissions);
        var newRefreshToken = await GenerateRefreshTokenAsync();

        // ذخیره توکن جدید
        var newUserToken = new UserToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(SystemConstants.RefreshTokenExpirationDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _userTokenRepository.SaveTokenAsync(newUserToken);

        return (newAccessToken, newRefreshToken);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در تازه‌سازی توکن");
        return null;
    }
}
}
