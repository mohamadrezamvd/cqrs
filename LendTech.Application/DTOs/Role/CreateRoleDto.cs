using System;
namespace LendTech.Application.DTOs.Role;
/// <summary>
/// DTO ایجاد نقش
/// </summary>
public class CreateRoleDto
{
public string Name { get; set; } = null!;
public string? Description { get; set; }
public List<Guid> PermissionGroupIds { get; set; } = new();
}
