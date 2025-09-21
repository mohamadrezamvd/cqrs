// ChangePasswordCommandHandler.cs
using Ardalis.Result;
using MediatR;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.Application.Services.Interfaces;
using LendTech.SharedKernel.Helpers;
using LendTech.SharedKernel.Enums;
using Microsoft.Extensions.Logging;
using LendTech.Application.Commands.User;

namespace LendTech.Application.Handlers.Commands.User;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IAuditLogRepository _auditLogRepository;
	private readonly ILogger<ChangePasswordCommandHandler> _logger;

	public ChangePasswordCommandHandler(
		IUserRepository userRepository,
		IAuditLogRepository auditLogRepository,
		ILogger<ChangePasswordCommandHandler> logger)
	{
		_userRepository = userRepository;
		_auditLogRepository = auditLogRepository;
		_logger = logger;
	}

	public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
			if (user == null)
				return Result.NotFound("کاربر یافت نشد");

			// ذخیره رمز جدید
			user.PasswordHash = PasswordHelper.HashPassword(request.ChangeRequest.NewPassword);
			user.ModifiedAt = DateTime.UtcNow;
			user.ModifiedBy = request.ModifiedBy;
			await _userRepository.UpdateAsync(user, cancellationToken);

			await _auditLogRepository.LogAsync(
				user.OrganizationId,
				nameof(user),
				user.Id.ToString(),
				AuditAction.Update,
				request.ModifiedBy != null ? Guid.Parse(request.ModifiedBy) : null,
				newValues: new { PasswordChanged = true },
				cancellationToken: cancellationToken);

			_logger.LogInformation("کاربر {UserId} رمز عبور خود را تغییر داد", user.Id);
			return Result.Success();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "خطا در تغییر رمز عبور کاربر {UserId}", request.UserId);
			return Result.Error("خطا در تغییر رمز عبور");
		}
	}
}