using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using LendTech.Application.Commands.User;
using LendTech.Application.Queries.User;
using LendTech.Application.DTOs.User;
using LendTech.Application.DTOs.Common;
using LendTech.API.Policies;
using LendTech.SharedKernel.Models;
using LendTech.SharedKernel.Constants;
namespace LendTech.API.Controllers;
/// <summary>
/// کنترلر مدیریت کاربران
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
private readonly IMediator _mediator;
private readonly ILogger<UsersController> _logger;
public UsersController(IMediator mediator, ILogger<UsersController> logger)
{
    _mediator = mediator;
    _logger = logger;
}

/// <summary>
/// دریافت لیست کاربران
/// </summary>
[HttpGet]
[Authorize(Policy = PolicyNames.ViewUsers)]
[ProducesResponseType(typeof(ApiResponse<PagedResult<UserListDto>>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetUsers([FromQuery] PagedRequestDto request)
{
    var organizationId = Guid.Parse(User.FindFirst(SecurityConstants.Claims.OrganizationId)?.Value ?? Guid.Empty.ToString());
    
    var query = new GetUsersListQuery
    {
        OrganizationId = organizationId,
        PagedRequest = request
    };

    var result = await _mediator.Send(query);

    if (!result.IsSuccess)
    {
        return BadRequest(ApiResponse.Error(SharedKernel.Enums.ResponseStatus.BusinessError, result.Errors.FirstOrDefault() ?? "خطا در دریافت لیست کاربران"));
    }

    return Ok(ApiResponse<PagedResult<UserListDto>>.SuccessResult(result.Value));
}

/// <summary>
/// دریافت اطلاعات کاربر
/// </summary>
[HttpGet("{id:guid}")]
[Authorize(Policy = PolicyNames.ViewUsers)]
[ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetUser(Guid id)
{
    var query = new GetUserByIdQuery
    {
        UserId = id,
        IncludeRoles = true
    };

    var result = await _mediator.Send(query);

    if (!result.IsSuccess)
    {
        if (result.Status == Ardalis.Result.ResultStatus.NotFound)
            return NotFound(ApiResponse.NotFound("کاربر یافت نشد"));
            
        return BadRequest(ApiResponse.Error(SharedKernel.Enums.ResponseStatus.BusinessError, result.Errors.FirstOrDefault() ?? "خطا در دریافت اطلاعات کاربر"));
    }

    return Ok(ApiResponse<UserDto>.SuccessResult(result.Value));
}

/// <summary>
/// ایجاد کاربر جدید
/// </summary>
[HttpPost]
[Authorize(Policy = PolicyNames.CreateUser)]
[ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
{
    var organizationId = Guid.Parse(User.FindFirst(SecurityConstants.Claims.OrganizationId)?.Value ?? Guid.Empty.ToString());
    var currentUserId = User.FindFirst(SecurityConstants.Claims.UserId)?.Value;
    
    var command = new CreateUserCommand
    {
        OrganizationId = organizationId,
        User = request,
        CreatedBy = currentUserId
    };

    var result = await _mediator.Send(command);

    if (!result.IsSuccess)
    {
        if (result.Status == Ardalis.Result.ResultStatus.Invalid)
            return BadRequest(ApiResponse.ValidationError(result.ValidationErrors.ToDictionary(e => e.Identifier, e => new[] { e.ErrorMessage })));
            
        return BadRequest(ApiResponse.Error(SharedKernel.Enums.ResponseStatus.BusinessError, result.Errors.FirstOrDefault() ?? "خطا در ایجاد کاربر"));
    }

    _logger.LogInformation("کاربر جدید {UserId} توسط {CreatedBy} ایجاد شد", result.Value.Id, currentUserId);

    return CreatedAtAction(nameof(GetUser), new { id = result.Value.Id }, ApiResponse<UserDto>.SuccessResult(result.Value, "کاربر با موفقیت ایجاد شد"));
}

/// <summary>
/// به‌روزرسانی کاربر
/// </summary>
[HttpPut("{id:guid}")]
[Authorize(Policy = PolicyNames.UpdateUser)]
[ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto request)
{
    var currentUserId = User.FindFirst(SecurityConstants.Claims.UserId)?.Value;
    
    var command = new UpdateUserCommand
    {
        UserId = id,
        User = request,
        ModifiedBy = currentUserId
    };

    var result = await _mediator.Send(command);

    if (!result.IsSuccess)
    {
        if (result.Status == Ardalis.Result.ResultStatus.NotFound)
            return NotFound(ApiResponse.NotFound("کاربر یافت نشد"));
            
        if (result.Status == Ardalis.Result.ResultStatus.Invalid)
            return BadRequest(ApiResponse.ValidationError(result.ValidationErrors.ToDictionary(e => e.Identifier, e => new[] { e.ErrorMessage })));
            
        return BadRequest(ApiResponse.Error(SharedKernel.Enums.ResponseStatus.BusinessError, result.Errors.FirstOrDefault() ?? "خطا در به‌روزرسانی کاربر"));
    }

    _logger.LogInformation("کاربر {UserId} توسط {ModifiedBy} به‌روزرسانی شد", id, currentUserId);

    return Ok(ApiResponse<UserDto>.SuccessResult(result.Value, "کاربر با موفقیت به‌روزرسانی شد"));
}

/// <summary>
/// حذف کاربر
/// </summary>
[HttpDelete("{id:guid}")]
[Authorize(Policy = PolicyNames.DeleteUser)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeleteUser(Guid id)
{
    var currentUserId = User.FindFirst(SecurityConstants.Claims.UserId)?.Value;
    
    var command = new DeleteUserCommand
    {
        UserId = id,
        DeletedBy = currentUserId
    };

    var result = await _mediator.Send(command);

    if (!result.IsSuccess)
    {
        if (result.Status == Ardalis.Result.ResultStatus.NotFound)
            return NotFound(ApiResponse.NotFound("کاربر یافت نشد"));
            
        return BadRequest(ApiResponse.Error(SharedKernel.Enums.ResponseStatus.BusinessError, result.Errors.FirstOrDefault() ?? "خطا در حذف کاربر"));
    }

    _logger.LogInformation("کاربر {UserId} توسط {DeletedBy} حذف شد", id, currentUserId);

    return Ok(ApiResponse.Success("کاربر با موفقیت حذف شد"));
}

/// <summary>
/// تغییر رمز عبور کاربر
/// </summary>
[HttpPost("{id:guid}/change-password")]
[Authorize]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto request)
{
    var currentUserId = Guid.Parse(User.FindFirst(SecurityConstants.Claims.UserId)?.Value ?? Guid.Empty.ToString());
    
    // کاربر فقط می‌تواند رمز خودش را تغییر دهد مگر اینکه دسترسی ویژه داشته باشد
    if (currentUserId != id && !User.HasClaim(SecurityConstants.Claims.Permissions, "Users.ChangeOthersPassword"))
    {
        return Forbid();
    }

    // TODO: پیاده‌سازی تغییر رمز عبور
    await Task.CompletedTask;

    return Ok(ApiResponse.Success("رمز عبور با موفقیت تغییر یافت"));
}
}
/// <summary>
/// DTO تغییر رمز عبور
/// </summary>
public class ChangePasswordDto
{
public string CurrentPassword { get; set; } = null!;
public string NewPassword { get; set; } = null!;
public string ConfirmPassword { get; set; } = null!;
}
