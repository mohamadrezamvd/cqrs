namespace LendTech.Application.DTOs.User;

/// <summary>
/// DTO تغییر رمز عبور
/// </summary>
public class ChangePasswordDto
{
	public string NewPassword { get; set; } = null!;
	public string ConfirmPassword { get; set; } = null!;
}
