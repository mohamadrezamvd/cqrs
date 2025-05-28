using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using LendTech.Application.Commands.Auth;
using LendTech.Application.DTOs.Auth;
using LendTech.SharedKernel.Models;
using LendTech.SharedKernel.Constants;
namespace LendTech.API.Controllers;
/// <summary>
/// کنترلر احراز هویت
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
private readonly IMediator _mediator;
private readonly ILogger<AuthController> _logger;
public AuthController(IMediator mediator, ILogger<AuthController> logger)
{
    _mediator = mediator;
    _logger = logger;
}

/// <summary>
/// ورود به سیستم
/// </summary>
/// <param name="request">اطلاعات ورود</param>
/// <returns>توکن دسترسی</returns>
[HttpPost("login")]
[AllowAnonymous]
[ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
{
    var command = new LoginCommand
    {
        LoginRequest = request,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        UserAgent = Request.Headers[SecurityConstants.Headers.UserAgent].ToString()
    };

    var result = await _mediator.Send(command);

    if (!result.IsSuccess)
    {
        return result.Status switch
        {
            Ardalis.Result.ResultStatus.Unauthorized => Unauthorized(ApiResponse.Unauthorized(result.Errors.FirstOrDefault() ?? ValidationMessages.InvalidCredentials)),
            Ardalis.Result.ResultStatus.Invalid => BadRequest(ApiResponse.ValidationError(result.ValidationErrors.ToDictionary(e => e.Identifier, e => new[] { e.ErrorMessage }))),
            _ => BadRequest(ApiResponse.Error(SharedKernel.Enums.ResponseStatus.BusinessError, result.Errors.FirstOrDefault() ?? "خطا در ورود"))
        };
    }

    _logger.LogInformation("کاربر {UserId} با موفقیت وارد شد", result.Value.User.Id);

    return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result.Value, "ورود موفقیت‌آمیز"));
}

/// <summary>
/// خروج از سیستم
/// </summary>
[HttpPost("logout")]
[Authorize]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
public async Task<IActionResult> Logout()
{
    // TODO: ابطال توکن در Redis
    var userId = User.FindFirst(SecurityConstants.Claims.UserId)?.Value;
    
    _logger.LogInformation("کاربر {UserId} از سیستم خارج شد", userId);
    
    return Ok(ApiResponse.Success("خروج موفقیت‌آمیز"));
}

/// <summary>
/// تازه‌سازی توکن
/// </summary>
/// <param name="refreshToken">توکن بازیابی</param>
/// <returns>توکن جدید</returns>
[HttpPost("refresh-token")]
[AllowAnonymous]
[ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
{
    // TODO: پیاده‌سازی Refresh Token
    await Task.CompletedTask;
    
    return Unauthorized(ApiResponse.Unauthorized("توکن نامعتبر است"));
}

/// <summary>
/// دریافت اطلاعات کاربر جاری
/// </summary>
[HttpGet("me")]
[Authorize]
[ProducesResponseType(typeof(ApiResponse<UserInfoDto>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetCurrentUser()
{
    // TODO: دریافت اطلاعات کامل کاربر از دیتابیس
    var userInfo = new UserInfoDto
    {
        Id = Guid.Parse(User.FindFirst(SecurityConstants.Claims.UserId)?.Value ?? Guid.Empty.ToString()),
        Username = User.FindFirst(SecurityConstants.Claims.Username)?.Value ?? "",
        Email = User.FindFirst(SecurityConstants.Claims.Email)?.Value ?? "",
        FullName = "", // از دیتابیس
        Roles = User.FindAll(SecurityConstants.Claims.Roles).Select(c => c.Value).ToList(),
        Permissions = User.FindAll(SecurityConstants.Claims.Permissions).Select(c => c.Value).ToList()
    };

    await Task.CompletedTask;
    
    return Ok(ApiResponse<UserInfoDto>.SuccessResult(userInfo));
}
}
