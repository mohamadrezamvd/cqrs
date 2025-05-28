using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LendTech.Application.Services.Interfaces;
/// <summary>
/// سرویس مدیریت دسترسی‌ها
/// </summary>
public interface IPermissionService
{
/// <summary>
/// بررسی دسترسی کاربر
/// </summary>
Task<bool> HasPermissionAsync(Guid userId, string permissionCode);
/// <summary>
/// بررسی چند دسترسی کاربر
/// </summary>
Task<bool> HasAnyPermissionAsync(Guid userId, params string[] permissionCodes);

/// <summary>
/// بررسی همه دسترسی‌های کاربر
/// </summary>
Task<bool> HasAllPermissionsAsync(Guid userId, params string[] permissionCodes);

/// <summary>
/// دریافت دسترسی‌های کاربر
/// </summary>
Task<List<string>> GetUserPermissionsAsync(Guid userId);

/// <summary>
/// بازخوانی کش دسترسی‌های کاربر
/// </summary>
Task RefreshUserPermissionsCacheAsync(Guid userId);

/// <summary>
/// پاک کردن کش دسترسی‌های کاربر
/// </summary>
Task ClearUserPermissionsCacheAsync(Guid userId);
}
