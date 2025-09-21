using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using LendTech.Application.DTOs.Auth;
namespace LendTech.Application.Services.Interfaces;
/// <summary>
/// سرویس مدیریت توکن
/// </summary>
public interface ITokenService
{
/// <summary>
/// تولید توکن دسترسی
/// </summary>
Task<string> GenerateAccessTokenAsync(Guid userId, Guid organizationId, List<string> roles, List<string> permissions);
/// <summary>
/// تولید توکن بازیابی
/// </summary>
Task<string> GenerateRefreshTokenAsync();

/// <summary>
/// اعتبارسنجی توکن
/// </summary>
Task<ClaimsPrincipal?> ValidateTokenAsync(string token);

/// <summary>
/// دریافت اطلاعات از توکن
/// </summary>
Task<UserInfoDto?> GetUserInfoFromTokenAsync(string token);

/// <summary>
/// ابطال توکن
/// </summary>
Task RevokeTokenAsync(string token);

/// <summary>
/// تازه‌سازی توکن
/// </summary>
Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken);
}
