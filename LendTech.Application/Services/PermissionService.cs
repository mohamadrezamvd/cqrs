using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LendTech.Application.Services.Interfaces;
using LendTech.Infrastructure.Redis.Interfaces;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Extensions;
namespace LendTech.Application.Services;
/// <summary>
/// سرویس مدیریت دسترسی‌ها
/// </summary>
public class PermissionService : IPermissionService
{
private readonly IUserRepository _userRepository;
private readonly ICacheService _cacheService;
private readonly ILogger<PermissionService> _logger;
public PermissionService(
    IUserRepository userRepository,
    ICacheService cacheService,
    ILogger<PermissionService> logger)
{
    _userRepository = userRepository;
    _cacheService = cacheService;
    _logger = logger;
}

/// <inheritdoc />
public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode)
{
    var permissions = await GetUserPermissionsAsync(userId);
    return permissions.Contains(permissionCode);
}

/// <inheritdoc />
public async Task<bool> HasAnyPermissionAsync(Guid userId, params string[] permissionCodes)
{
    var permissions = await GetUserPermissionsAsync(userId);
    return permissionCodes.Any(code => permissions.Contains(code));
}

/// <inheritdoc />
public async Task<bool> HasAllPermissionsAsync(Guid userId, params string[] permissionCodes)
{
    var permissions = await GetUserPermissionsAsync(userId);
    return permissionCodes.All(code => permissions.Contains(code));
}

/// <inheritdoc />
public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
{
    try
    {
        var cacheKey = CacheKeys.UserPermissions(userId.ToString());
        
        // دریافت از کش
        var cachedPermissions = await _cacheService.GetAsync<List<string>>(cacheKey);
        
        if (cachedPermissions != null)
        {
            _logger.LogDebug("دسترسی‌های کاربر {UserId} از کش بازیابی شد", userId);
            return cachedPermissions;
        }

        // دریافت از دیتابیس
        var permissions = await _userRepository.GetUserPermissionsAsync(userId);
        
        // ذخیره در کش
        await _cacheService.SetAsync(
            cacheKey,
            permissions,
            TimeSpan.FromMinutes(CacheKeys.DefaultCacheMinutes));

        _logger.LogDebug("دسترسی‌های کاربر {UserId} از دیتابیس بازیابی و در کش ذخیره شد", userId);
        
        return permissions;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در دریافت دسترسی‌های کاربر {UserId}", userId);
        return new List<string>();
    }
}

/// <inheritdoc />
public async Task RefreshUserPermissionsCacheAsync(Guid userId)
{
    try
    {
        var cacheKey = CacheKeys.UserPermissions(userId.ToString());
        
        // حذف از کش
        await _cacheService.RemoveAsync(cacheKey);
        
        // بارگذاری مجدد
        await GetUserPermissionsAsync(userId);
        
        _logger.LogInformation("کش دسترسی‌های کاربر {UserId} به‌روزرسانی شد", userId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در به‌روزرسانی کش دسترسی‌های کاربر {UserId}", userId);
    }
}

/// <inheritdoc />
public async Task ClearUserPermissionsCacheAsync(Guid userId)
{
    try
    {
        var cacheKey = CacheKeys.UserPermissions(userId.ToString());
        await _cacheService.RemoveAsync(cacheKey);
        
        // همچنین کش نقش‌ها را پاک کنیم
        var rolesKey = CacheKeys.UserRoles(userId.ToString());
        await _cacheService.RemoveAsync(rolesKey);
        
        _logger.LogInformation("کش دسترسی‌ها و نقش‌های کاربر {UserId} پاک شد", userId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پاک کردن کش کاربر {UserId}", userId);
    }
}
}
