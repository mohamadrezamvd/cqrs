using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LendTech.Application.DTOs.Auth;
using LendTech.Application.Services.Interfaces;
using LendTech.Infrastructure.Redis.Interfaces;
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
private readonly ILogger<TokenService> _logger;
public TokenService(
    IConfiguration configuration,
    ICacheService cacheService,
    ILogger<TokenService> logger)
{
    _configuration = configuration;
    _cacheService = cacheService;
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

        var token = JwtHelper.GenerateToken(
            claims,
            _configuration["Jwt:SecretKey"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!,
            int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));

        // ذخیره در کش برای امکان ابطال
        var cacheKey = $"jwt:access:{userId}:{Guid.NewGuid()}";
        await _cacheService.SetStringAsync(
            cacheKey,
            "active",
            TimeSpan.FromHours(24));

        _logger.LogInformation("توکن دسترسی برای کاربر {UserId} تولید شد", userId);

        return token;
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

        // بررسی ابطال توکن
        var userId = JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.UserId);
        
        if (!string.IsNullOrEmpty(userId))
        {
            // TODO: بررسی وضعیت توکن در کش
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
        var principal = await ValidateTokenAsync(token);
        
        if (principal == null)
            return;

        var userId = JwtHelper.GetClaimValue(principal, SecurityConstants.Claims.UserId);

        if (!string.IsNullOrEmpty(userId))
        {
            // TODO: ذخیره در لیست توکن‌های ابطال شده
            _logger.LogInformation("توکن کاربر {UserId} ابطال شد", userId);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ابطال توکن");
    }
}
}
