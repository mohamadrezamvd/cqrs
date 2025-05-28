using System;
namespace LendTech.Application.DTOs.User;
/// <summary>
/// DTO به‌روزرسانی کاربر
/// </summary>
public class UpdateUserDto
{
public string Email { get; set; } = null!;
public string? FirstName { get; set; }
public string? LastName { get; set; }
public string? MobileNumber { get; set; }
public bool IsActive { get; set; }
public List<Guid>? RoleIds { get; set; }
}
