using System;
using Ardalis.Result;
using MediatR;
using LendTech.Application.DTOs.User;
namespace LendTech.Application.Commands.User;
/// <summary>
/// دستور ایجاد کاربر
/// </summary>
public class CreateUserCommand : IRequest<Result<UserDto>>
{
public Guid OrganizationId { get; set; }
public CreateUserDto User { get; set; } = null!;
public string? CreatedBy { get; set; }
}
