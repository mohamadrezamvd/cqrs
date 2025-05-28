using System;
namespace LendTech.Application.DTOs.Permission;
/// <summary>
/// DTO دسترسی
/// </summary>
public class PermissionDto
{
public Guid Id { get; set; }
public Guid PermissionGroupId { get; set; }
public string Name { get; set; } = null!;
public string? Description { get; set; }
public string Code { get; set; } = null!;
public string GroupName { get; set; } = null!;
}
