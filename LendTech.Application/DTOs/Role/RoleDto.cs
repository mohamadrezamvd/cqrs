using System;
using LendTech.Application.DTOs.Permission;

namespace LendTech.Application.DTOs.Role;
/// <summary>
/// DTO نقش
/// </summary>
public class RoleDto
{
public Guid Id { get; set; }
public Guid OrganizationId { get; set; }
public string Name { get; set; } = null!;
public string? Description { get; set; }
public bool IsSystemRole { get; set; }
public DateTime CreatedAt { get; set; }
public int UserCount { get; set; }
public List<PermissionGroupDto> PermissionGroups { get; set; } = new();
}
