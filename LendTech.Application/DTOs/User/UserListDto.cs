using System;
namespace LendTech.Application.DTOs.User;
/// <summary>
/// DTO لیست کاربران
/// </summary>
public class UserListDto
{
public Guid Id { get; set; }
public string Username { get; set; } = null!;
public string Email { get; set; } = null!;
public string FullName { get; set; } = null!;
public bool IsActive { get; set; }
public bool IsLocked { get; set; }
public int RoleCount { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime? LastLoginAt { get; set; }
}
