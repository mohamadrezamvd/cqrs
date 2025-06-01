using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using LendTech.Application.Commands.User;
using LendTech.Application.DTOs.User;
using LendTech.Application.Services.Interfaces;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Enums;
namespace LendTech.Application.Handlers.Commands.User;
/// <summary>
/// هندلر دستور به‌روزرسانی کاربر
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
private readonly IUserRepository _userRepository;
private readonly IPermissionService _permissionService;
private readonly IAuditLogRepository _auditLogRepository;
private readonly IMapper _mapper;
private readonly ILogger<UpdateUserCommandHandler> _logger;
public UpdateUserCommandHandler(
    IUserRepository userRepository,
    IPermissionService permissionService,
    IAuditLogRepository auditLogRepository,
    IMapper mapper,
    ILogger<UpdateUserCommandHandler> logger)
{
    _userRepository = userRepository;
    _permissionService = permissionService;
    _auditLogRepository = auditLogRepository;
    _mapper = mapper;
    _logger = logger;
}

public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
{
    try
    {
        // دریافت کاربر موجود
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            return Result<UserDto>.NotFound($"کاربر با شناسه {request.UserId} یافت نشد");
        }

        // ذخیره مقادیر قبلی برای Audit
        var oldValues = new
        {
            user.Email,
            user.FirstName,
            user.LastName,
            user.MobileNumber,
            user.IsActive
        };

        // بررسی تکراری نبودن ایمیل
        if (user.Email != request.User.Email)
        {
            if (await _userRepository.IsEmailExistsAsync(
                request.User.Email, 
                user.OrganizationId, 
                user.Id, 
                cancellationToken))
            {
                return Result<UserDto>.Invalid(
                    new ValidationError(ValidationMessages.Duplicate.Replace("{0}", "ایمیل")));
            }
        }

        // به‌روزرسانی فیلدها
        _mapper.Map(request.User, user);
        user.ModifiedAt = DateTime.UtcNow;
        user.ModifiedBy = request.ModifiedBy;

        // ذخیره تغییرات
        await _userRepository.UpdateAsync(user, cancellationToken);

        // به‌روزرسانی نقش‌ها در صورت نیاز
        if (request.User.RoleIds != null)
        {
            // TODO: پیاده‌سازی به‌روزرسانی UserRoles
        }

        // پاکسازی کش دسترسی‌ها
        await _permissionService.ClearUserPermissionsCacheAsync(user.Id);

        // ثبت در Audit Log
        await _auditLogRepository.LogAsync(
            user.OrganizationId,
            nameof(User),
            user.Id.ToString(),
            AuditAction.Update,
            request.ModifiedBy != null ? Guid.Parse(request.ModifiedBy) : null,
            oldValues: oldValues,
            newValues: new
            {
                user.Email,
                user.FirstName,
                user.LastName,
                user.MobileNumber,
                user.IsActive
            },
            cancellationToken: cancellationToken);

        _logger.LogInformation("کاربر {UserId} با موفقیت به‌روزرسانی شد", user.Id);

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(userDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در به‌روزرسانی کاربر {UserId}", request.UserId);
        return Result<UserDto>.Error("خطا در به‌روزرسانی کاربر");
    }
}
}
