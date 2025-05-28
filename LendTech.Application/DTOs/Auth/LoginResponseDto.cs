using System;
namespace LendTech.Application.DTOs.Auth;
/// <summary>
/// DTO پاسخ ورود
/// </summary>
public class LoginResponseDto
{
public string AccessToken { get; set; } = null!;
public string RefreshToken { get; set; } = null!;
public DateTime ExpiresAt { get; set; }
public UserInfoDto User { get; set; } = null!;
}
/// <summary>
/// اطلاعات کاربر در پاسخ ورود
/// </summary>
public class UserInfoDto
{
public Guid Id { get; set; }
public string Username { get; set; } = null!;
public string Email { get; set; } = null!;
public string FullName { get; set; } = null!;
public List<string> Roles { get; set; } = new();
public List<string> Permissions { get; set; } = new();
}
