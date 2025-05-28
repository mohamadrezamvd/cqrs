using System;
using Ardalis.Result;
using MediatR;
namespace LendTech.Application.Commands.User;
/// <summary>
/// دستور حذف کاربر
/// </summary>
public class DeleteUserCommand : IRequest<Result>
{
public Guid UserId { get; set; }
public string? DeletedBy { get; set; }
}
