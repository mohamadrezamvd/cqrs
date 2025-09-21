using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using LendTech.Application.DTOs.User;
using LendTech.Application.Queries.User;
using LendTech.Infrastructure.Repositories.Interfaces;
namespace LendTech.Application.Handlers.Queries.User;
/// <summary>
/// هندلر کوئری دریافت کاربر با شناسه
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
private readonly IUserRepository _userRepository;
private readonly IMapper _mapper;
private readonly ILogger<GetUserByIdQueryHandler> _logger;
public GetUserByIdQueryHandler(
    IUserRepository userRepository,
    IMapper mapper,
    ILogger<GetUserByIdQueryHandler> logger)
{
    _userRepository = userRepository;
    _mapper = mapper;
    _logger = logger;
}

public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
{
    try
    {
        var user = request.IncludeRoles 
            ? await _userRepository.GetWithRolesAsync(request.UserId, cancellationToken)
            : await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.NotFound($"کاربر با شناسه {request.UserId} یافت نشد");
        }

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.Success(userDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "خطا در دریافت اطلاعات کاربر {UserId}", request.UserId);
        return Result<UserDto>.Error("خطا در دریافت اطلاعات کاربر");
    }
}
}
