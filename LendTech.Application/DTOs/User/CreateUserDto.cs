using System;
namespace LendTech.Application.DTOs.User;
/// <summary>
/// DTO ایجاد کاربر
/// </summary>
public class CreateUserDto
{
public string Username { get; set; } = null!;
public string Email { get; set; } = null!;
public string Password { get; set; } = null!;
public string? FirstName { get; set; }
public string? LastName { get; set; }
public string? MobileNumber { get; set; }
public bool IsActive { get; set; } = true;
public List<Guid>? RoleIds { get; set; }
}
