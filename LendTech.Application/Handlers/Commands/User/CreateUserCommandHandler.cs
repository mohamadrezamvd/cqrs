using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using LendTech.Application.DTOs.User;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Helpers;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Enums;
using LendTech.Application.Commands.User;

namespace LendTech.Application.Handlers.Commands.User;
/// <summary>
/// هندلر دستور ایجاد کاربر
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
private readonly IUserRepository _userRepository;
private readonly IRoleRepository _roleRepository;
private readonly IAuditLogRepository _auditLogRepository;
private readonly IMapper _mapper;
private readonly ILogger<CreateUserCommandHandler> _logger;
public CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IAuditLogRepository auditLogRepository,
    IMapper mapper,
    ILogger<CreateUserCommandHandler> logger)
{
    _userRepository = userRepository;
    _roleRepository = roleRepository;
    _auditLogRepository = auditLogRepository;
    _mapper = mapper;
    _logger = logger;
}

public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
{
    try
    {
        // بررسی تکراری نبودن نام کاربری
        if (await _userRepository.IsUsernameExistsAsync(request.User.Username, request.OrganizationId, cancellationToken: cancellationToken))
        {
            return Result<UserDto>.Invalid(new ValidationError(ValidationMessages.Duplicate.Replace("{0}", "نام کاربری")));
        }

        // بررسی تکراری نبودن ایمیل
        if (await _userRepository.IsEmailExistsAsync(request.User.Email, request.OrganizationId, cancellationToken: cancellationToken))
        {
            return Result<UserDto>.Invalid(new ValidationError(ValidationMessages.Duplicate.Replace("{0}", "ایمیل")));
        }

        // ایجاد موجودیت کاربر
        var user = new Database.Entities.User
        {
            OrganizationId = request.OrganizationId,
            Username = request.User.Username,
            Email = request.User.Email,
            PasswordHash = PasswordHelper.HashPassword(request.User.Password),
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            MobileNumber = request.User.MobileNumber,
            IsActive = request.User.IsActive,
            CreatedBy = request.CreatedBy
        };

        // ذخیره کاربر
        user = await _userRepository.AddAsync(user, cancellationToken);

        // اضافه کردن نقش‌ها
        if (request.User.RoleIds?.Any() == true)
        {
            var userRoles = new List<UserRole>();
            foreach (var roleId in request.User.RoleIds)
            {
                userRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId,
                    CreatedBy = request.CreatedBy
                });
            }
            // TODO: اضافه کردن UserRoles
        }

        // ثبت در Audit Log
        await _auditLogRepository.LogAsync(
            request.OrganizationId,
            nameof(User),
            user.Id.ToString(),
            AuditAction.Create,
            request.CreatedBy != null ? Guid.Parse(request.CreatedBy) : null,
            newValues: user,
            cancellationToken: cancellationToken);

        _logger.LogInformation("کاربر جدید با شناسه {UserId} ایجاد شد", user.Id);

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(userDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در ایجاد کاربر");
        return Result<UserDto>.Error("خطا در ایجاد کاربر");
    }
}
}
