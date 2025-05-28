using System;
using Ardalis.Result;
using MediatR;
using LendTech.Application.DTOs.User;
namespace LendTech.Application.Commands.User;
/// <summary>
/// دستور به‌روزرسانی کاربر
/// </summary>
public class UpdateUserCommand : IRequest<Result<UserDto>>
{
public Guid UserId { get; set; }
public UpdateUserDto User { get; set; } = null!;
public string? ModifiedBy { get; set; }
}
