namespace LendTech.Application.DTOs.Auth;
/// <summary>
/// DTO درخواست ورود
/// </summary>
public class LoginRequestDto
{
public string Username { get; set; } = null!;
public string Password { get; set; } = null!;
public Guid OrganizationId { get; set; }
public bool RememberMe { get; set; }
}
