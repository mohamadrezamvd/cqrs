using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LendTech.Application.Commands.Auth;
using LendTech.Application.DTOs.Auth;
using LendTech.Application.Services.Interfaces;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Models;
using MediatR;

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
    private readonly ITokenService _tokenService;
    private readonly IMediator _mediator;
	private readonly IUserTokenRepository _userTokenRepository;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
        ITokenService tokenService,
        IUserTokenRepository userTokenRepository,
        ILogger<AuthController> logger, IMediator mediator, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userTokenRepository = userTokenRepository;
        _logger = logger;
        _mediator = mediator;
        _configuration = configuration;
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
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await _mediator.Send(request);

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
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        await _tokenService.RevokeTokenAsync(token);

        var userId = User.FindFirst(SecurityConstants.Claims.UserId)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            // ابطال همه توکن‌های کاربر
            await _userTokenRepository.RevokeAllUserTokensAsync(Guid.Parse(userId), "System", "Logout");
            _logger.LogInformation("کاربر {UserId} از سیستم خارج شد", userId);
        }

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
        var result = await _tokenService.RefreshTokenAsync(refreshToken);
        if (result == null)
        {
            return Unauthorized(ApiResponse.Unauthorized("توکن نامعتبر است"));
        }

        var (accessToken, newRefreshToken) = result.Value;
        var userInfo = await _tokenService.GetUserInfoFromTokenAsync(accessToken);

        if (userInfo == null)
        {
            return Unauthorized(ApiResponse.Unauthorized("خطا در بازیابی اطلاعات کاربر"));
        }

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            User = userInfo
        };

        return Ok(ApiResponse<LoginResponseDto>.SuccessResult(response, "تازه‌سازی توکن موفقیت‌آمیز"));
    }

    /// <summary>
    /// دریافت اطلاعات کاربر جاری
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var userInfo = await _tokenService.GetUserInfoFromTokenAsync(token);

        if (userInfo == null)
        {
            return Unauthorized(ApiResponse.Unauthorized("خطا در بازیابی اطلاعات کاربر"));
        }

        return Ok(ApiResponse<UserInfoDto>.SuccessResult(userInfo, "دریافت اطلاعات موفقیت‌آمیز"));
    }
}
