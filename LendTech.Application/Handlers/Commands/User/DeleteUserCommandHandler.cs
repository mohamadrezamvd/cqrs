using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using LendTech.Application.Commands.User;
using LendTech.Application.Services.Interfaces;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Enums;
using LendTech.SharedKernel.Exceptions;
namespace LendTech.Application.Handlers.Commands.User;
/// <summary>
/// هندلر دستور حذف کاربر
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
private readonly IUserRepository _userRepository;
private readonly IPermissionService _permissionService;
private readonly IAuditLogRepository _auditLogRepository;
private readonly IOutboxEventRepository _outboxEventRepository;
private readonly ILogger<DeleteUserCommandHandler> _logger;
public DeleteUserCommandHandler(
    IUserRepository userRepository,
    IPermissionService permissionService,
    IAuditLogRepository auditLogRepository,
    IOutboxEventRepository outboxEventRepository,
    ILogger<DeleteUserCommandHandler> logger)
{
    _userRepository = userRepository;
    _permissionService = permissionService;
    _auditLogRepository = auditLogRepository;
    _outboxEventRepository = outboxEventRepository;
    _logger = logger;
}

public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
{
    try
    {
        // دریافت کاربر
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            return Result.NotFound($"کاربر با شناسه {request.UserId} یافت نشد");
        }

        // بررسی عدم حذف خود
        if (request.DeletedBy == user.Id.ToString())
        {
            return Result.Error("کاربر نمی‌تواند خودش را حذف کند");
        }

        // حذف منطقی
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = request.DeletedBy;
        user.IsActive = false; // غیرفعال کردن حساب

        await _userRepository.UpdateAsync(user, cancellationToken);

        // پاکسازی کش
        await _permissionService.ClearUserPermissionsCacheAsync(user.Id);

        // ثبت رویداد در Outbox
        await _outboxEventRepository.CreateEventAsync(
            EventType.UserDeleted.ToString(),
            new
            {
                UserId = user.Id,
                Username = user.Username,
                DeletedAt = user.DeletedAt,
                DeletedBy = request.DeletedBy
            },
            user.OrganizationId,
            cancellationToken);

        // ثبت در Audit Log
        await _auditLogRepository.LogAsync(
            user.OrganizationId,
            nameof(User),
            user.Id.ToString(),
            AuditAction.Delete,
            request.DeletedBy != null ? Guid.Parse(request.DeletedBy) : null,
            oldValues: new { user.Username, user.Email },
            cancellationToken: cancellationToken);

        _logger.LogInformation("کاربر {UserId} با موفقیت حذف شد", user.Id);

        return Result.Success();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در حذف کاربر {UserId}", request.UserId);
        return Result.Error("خطا در حذف کاربر");
    }
}
}
