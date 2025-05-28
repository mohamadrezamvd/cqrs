using System;
namespace LendTech.Application.DTOs.Role;
/// <summary>
/// DTO به‌روزرسانی نقش
/// </summary>
public class UpdateRoleDto
{
public string Name { get; set; } = null!;
public string? Description { get; set; }
public List<Guid> PermissionGroupIds { get; set; } = new();
}
