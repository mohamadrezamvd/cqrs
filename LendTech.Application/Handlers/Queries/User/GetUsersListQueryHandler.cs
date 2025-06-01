using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LendTech.Application.DTOs.User;
using LendTech.Application.Queries.User;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Models;
namespace LendTech.Application.Handlers.Queries.User;
/// <summary>
/// هندلر کوئری دریافت لیست کاربران
/// </summary>
public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, Result<SharedKernel.Models.PagedResult<UserListDto>>>
{
private readonly IUserRepository _userRepository;
private readonly IMapper _mapper;
private readonly ILogger<GetUsersListQueryHandler> _logger;
public GetUsersListQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    ILogger<GetUsersListQueryHandler> logger)
{
    _userRepository = userRepository;
    _mapper = mapper;
    _logger = logger;
}

public async Task<Result<SharedKernel.Models.PagedResult<UserListDto>>> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
{
    try
    {
        // ایجاد کوئری پایه
        var query = _userRepository.GetQueryable()
            .Where(u => u.OrganizationId == request.OrganizationId);

        // اعمال فیلتر جستجو
        if (!string.IsNullOrWhiteSpace(request.PagedRequest.SearchTerm))
        {
            var searchTerm = request.PagedRequest.SearchTerm.ToLower();
            query = query.Where(u => 
                u.Username.ToLower().Contains(searchTerm) ||
                u.Email.ToLower().Contains(searchTerm) ||
                (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)));
        }

        // اعمال فیلترهای اضافی
        if (request.PagedRequest.Filters != null)
        {
            if (request.PagedRequest.Filters.TryGetValue("IsActive", out var isActiveStr) &&
                bool.TryParse(isActiveStr, out var isActive))
            {
                query = query.Where(u => u.IsActive == isActive);
            }

            if (request.PagedRequest.Filters.TryGetValue("IsLocked", out var isLockedStr) &&
                bool.TryParse(isLockedStr, out var isLocked))
            {
                query = query.Where(u => u.IsLocked == isLocked);
            }
        }

        // شمارش کل
        var totalCount = await query.CountAsync(cancellationToken);

        // مرتب‌سازی
        query = ApplySorting(query, request.PagedRequest.SortBy, request.PagedRequest.IsDescending);

        // صفحه‌بندی
        var users = await query
            .Skip((request.PagedRequest.PageNumber - 1) * request.PagedRequest.PageSize)
            .Take(request.PagedRequest.PageSize)
            .Include(u => u.UserRoles)
            .ToListAsync(cancellationToken);

        // نگاشت به DTO
        var userDtos = _mapper.Map<List<UserListDto>>(users);

        var result = new SharedKernel.Models.PagedResult<UserListDto>(
            userDtos,
            totalCount,
            request.PagedRequest.PageNumber,
            request.PagedRequest.PageSize);

        return Result<SharedKernel.Models.PagedResult<UserListDto>>.Success(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در دریافت لیست کاربران");
        return Result<SharedKernel.Models.PagedResult<UserListDto>>.Error("خطا در دریافت لیست کاربران");
    }
}

/// <summary>
/// اعمال مرتب‌سازی
/// </summary>
private IQueryable<Database.Entities.User> ApplySorting(
    IQueryable<Database.Entities.User> query, 
    string? sortBy, 
    bool isDescending)
{
    sortBy = sortBy?.ToLower() ?? "createdat";

    query = sortBy switch
    {
        "username" => isDescending 
            ? query.OrderByDescending(u => u.Username) 
            : query.OrderBy(u => u.Username),
        
        "email" => isDescending 
            ? query.OrderByDescending(u => u.Email) 
            : query.OrderBy(u => u.Email),
        
        "fullname" => isDescending 
            ? query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName) 
            : query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
        
        "isactive" => isDescending 
            ? query.OrderByDescending(u => u.IsActive) 
            : query.OrderBy(u => u.IsActive),
        
        _ => isDescending 
            ? query.OrderByDescending(u => u.CreatedAt) 
            : query.OrderBy(u => u.CreatedAt)
    };

    return query;
}
}
