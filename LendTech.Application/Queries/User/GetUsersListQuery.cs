using Ardalis.Result;
using MediatR;
using LendTech.Application.DTOs.Common;
using LendTech.Application.DTOs.User;
using LendTech.SharedKernel.Models;
namespace LendTech.Application.Queries.User;
/// <summary>
/// کوئری دریافت لیست کاربران
/// </summary>
public class GetUsersListQuery : IRequest<Result<SharedKernel.Models.PagedResult<UserListDto>>>
{
public Guid OrganizationId { get; set; }
public PagedRequestDto PagedRequest { get; set; } = new();
}
