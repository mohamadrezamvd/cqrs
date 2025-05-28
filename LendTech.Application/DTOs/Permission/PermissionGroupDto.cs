using System;
namespace LendTech.Application.DTOs.Permission;
/// <summary>
/// DTO گروه دسترسی
/// </summary>
public class PermissionGroupDto
{
public Guid Id { get; set; }
public string Name { get; set; } = null!;
public string? Description { get; set; }
public bool IsSystemGroup { get; set; }
public List<PermissionDto> Permissions { get; set; } = new();
}
