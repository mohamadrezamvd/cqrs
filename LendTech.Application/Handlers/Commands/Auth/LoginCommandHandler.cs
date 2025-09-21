using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using LendTech.Application.Commands.Auth;
using LendTech.Application.DTOs.Auth;
using LendTech.Application.Services.Interfaces;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Helpers;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Enums;
namespace LendTech.Application.Handlers.Commands.Auth;
/// <summary>
/// هندلر دستور ورود به سیستم
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
private readonly IUserRepository _userRepository;
private readonly ITokenService _tokenService;
private readonly IPermissionService _permissionService;
private readonly IAuditLogRepository _auditLogRepository;
private readonly ILogger<LoginCommandHandler> _logger;
public LoginCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPermissionService permissionService,
    IAuditLogRepository auditLogRepository,
    ILogger<LoginCommandHandler> logger)
{
    _userRepository = userRepository;
    _tokenService = tokenService;
    _permissionService = permissionService;
    _auditLogRepository = auditLogRepository;
    _logger = logger;
}

public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
{
    try
    {
        // دریافت کاربر
        var user = await _userRepository.GetByUsernameAsync(
            request.LoginRequest.Username, 
            request.LoginRequest.OrganizationId, 
            cancellationToken);

        if (user == null)
        {
            await LogFailedAttempt(request, "کاربر یافت نشد", cancellationToken);
            return Result<LoginResponseDto>.Unauthorized();
        }

        // بررسی قفل بودن حساب
        if (user.IsLocked && user.LockoutEnd > DateTime.UtcNow)
        {
            var remainingMinutes = (int)(user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes;
            return Result<LoginResponseDto>.Error(
                ValidationMessages.AccountLocked.Replace("{0}", remainingMinutes.ToString()));
        }

        // بررسی رمز عبور
        if (!PasswordHelper.VerifyPassword(request.LoginRequest.Password, user.PasswordHash))
        {
            await HandleFailedLogin(user, request, cancellationToken);
            return Result<LoginResponseDto>.Error(ValidationMessages.InvalidCredentials);
        }

        // بررسی فعال بودن حساب
        if (!user.IsActive)
        {
            await LogFailedAttempt(request, "حساب غیرفعال", cancellationToken);
            return Result<LoginResponseDto>.Error(ValidationMessages.InvalidCredentials);
        }

        // موفقیت در ورود
        await HandleSuccessfulLogin(user, cancellationToken);

        // دریافت نقش‌ها و دسترسی‌ها
        var roles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

        // تولید توکن‌ها
        var accessToken = await _tokenService.GenerateAccessTokenAsync(
            user.Id,
            user.OrganizationId,
            roles.Select(r => r.Name).ToList(),
            permissions);

        var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

        // ثبت در Audit Log
        await _auditLogRepository.LogAsync(
            user.OrganizationId,
            nameof(User),
            user.Id.ToString(),
            AuditAction.Login,
            user.Id,
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            cancellationToken: cancellationToken);

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = new UserInfoDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Roles = roles.Select(r => r.Name).ToList(),
                Permissions = permissions
            }
        };

        return Result<LoginResponseDto>.Success(response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در پردازش ورود کاربر");
        return Result<LoginResponseDto>.Error("خطا در ورود به سیستم");
    }
}

/// <summary>
/// مدیریت ورود ناموفق
/// </summary>
private async Task HandleFailedLogin(Database.Entities.User user, LoginCommand request, CancellationToken cancellationToken)
{
    await _userRepository.IncrementFailedAccessCountAsync(user.Id, cancellationToken);

    if (user.AccessFailedCount + 1 >= SystemConstants.MaxLoginAttempts)
    {
        var lockoutEnd = DateTime.UtcNow.AddMinutes(SystemConstants.AccountLockoutMinutes);
        await _userRepository.LockUserAsync(user.Id, lockoutEnd, cancellationToken);

        await _auditLogRepository.LogAsync(
            user.OrganizationId,
            nameof(User),
            user.Id.ToString(),
            AuditAction.AccountLocked,
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            cancellationToken: cancellationToken);
    }

    await LogFailedAttempt(request, "رمز عبور اشتباه", cancellationToken);
}

/// <summary>
/// مدیریت ورود موفق
/// </summary>
private async Task HandleSuccessfulLogin(Database.Entities.User user, CancellationToken cancellationToken)
{
    await _userRepository.ResetFailedAccessCountAsync(user.Id, cancellationToken);
    
    if (user.IsLocked)
    {
        await _userRepository.UnlockUserAsync(user.Id, cancellationToken);
    }
}

/// <summary>
/// ثبت تلاش ناموفق
/// </summary>
private async Task LogFailedAttempt(LoginCommand request, string reason, CancellationToken cancellationToken)
{
    await _auditLogRepository.LogAsync(
        request.LoginRequest.OrganizationId,
        "LoginAttempt",
        request.LoginRequest.Username,
        AuditAction.LoginFailed,
        newValues: new { Reason = reason },
        ipAddress: request.IpAddress,
        userAgent: request.UserAgent,
        cancellationToken: cancellationToken);

    _logger.LogWarning("تلاش ناموفق ورود برای {Username} - دلیل: {Reason}", 
        request.LoginRequest.Username, reason);
}
}
