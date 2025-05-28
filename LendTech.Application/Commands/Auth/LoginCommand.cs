using Ardalis.Result;
using MediatR;
using LendTech.Application.DTOs.Auth;
namespace LendTech.Application.Commands.Auth;
/// <summary>
/// دستور ورود به سیستم
/// </summary>
public class LoginCommand : IRequest<Result<LoginResponseDto>>
{
public LoginRequestDto LoginRequest { get; set; } = null!;
public string? IpAddress { get; set; }
public string? UserAgent { get; set; }
}
