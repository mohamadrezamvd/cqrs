using Ardalis.Result;
using LendTech.Application.DTOs.User;
using MediatR;

namespace LendTech.Application.Commands.User;

public class ChangePasswordCommand : IRequest<Result>
{
	public ChangePasswordDto ChangeRequest { get; set; } = null!;
	public Guid UserId { get; set; }
	public string? ModifiedBy { get; set; }
}