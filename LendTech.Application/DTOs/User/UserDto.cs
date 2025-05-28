using System;
namespace LendTech.Application.DTOs.User;
/// <summary>
/// DTO کاربر
/// </summary>
public class UserDto
{
public Guid Id { get; set; }
public Guid OrganizationId { get; set; }
public string Username { get; set; } = null!;
public string Email { get; set; } = null!;
public string? FirstName { get; set; }
public string? LastName { get; set; }
public string? MobileNumber { get; set; }
public bool IsActive { get; set; }
public bool IsLocked { get; set; }
public DateTime? LockoutEnd { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime? ModifiedAt { get; set; }
/// <summary>
/// نام کامل کاربر
/// </summary>
public string FullName => $"{FirstName} {LastName}".Trim();
}
