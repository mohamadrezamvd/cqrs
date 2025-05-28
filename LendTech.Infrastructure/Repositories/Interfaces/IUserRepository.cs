using System;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository کاربران
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// دریافت کاربر با نام کاربری
    /// </summary>
    Task<User?> GetByUsernameAsync(string username, Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کاربر با ایمیل
    /// </summary>
    Task<User?> GetByEmailAsync(string email, Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کاربر با شماره موبایل
    /// </summary>
    Task<User?> GetByMobileAsync(string mobileNumber, Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کاربر به همراه نقش‌ها
    /// </summary>
    Task<User?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کاربر به همراه دسترسی‌ها
    /// </summary>
    Task<User?> GetWithPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود نام کاربری
    /// </summary>
    Task<bool> IsUsernameExistsAsync(string username, Guid organizationId, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود ایمیل
    /// </summary>
    Task<bool> IsEmailExistsAsync(string email, Guid organizationId, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// افزایش تعداد تلاش ناموفق
    /// </summary>
    Task IncrementFailedAccessCountAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ریست کردن تعداد تلاش ناموفق
    /// </summary>
    Task ResetFailedAccessCountAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// قفل کردن کاربر
    /// </summary>
    Task LockUserAsync(Guid userId, DateTime lockoutEnd, CancellationToken cancellationToken = default);

    /// <summary>
    /// باز کردن قفل کاربر
    /// </summary>
    Task UnlockUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    Task ChangePasswordAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت دسترسی‌های کاربر
    /// </summary>
    Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت نقش‌های کاربر
    /// </summary>
    Task<List<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
