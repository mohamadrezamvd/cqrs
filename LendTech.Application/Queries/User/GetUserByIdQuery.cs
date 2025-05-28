using System;
using Ardalis.Result;
using MediatR;
using LendTech.Application.DTOs.User;
namespace LendTech.Application.Queries.User;
/// <summary>
/// کوئری دریافت کاربر با شناسه
/// </summary>
public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
public Guid UserId { get; set; }
public bool IncludeRoles { get; set; }
}
