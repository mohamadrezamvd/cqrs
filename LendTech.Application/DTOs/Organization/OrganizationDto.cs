using System;
using LendTech.SharedKernel.Models;
namespace LendTech.Application.DTOs.Organization;
/// <summary>
/// DTO سازمان
/// </summary>
public class OrganizationDto
{
public Guid Id { get; set; }
public string Name { get; set; } = null!;
public string Code { get; set; } = null!;
public bool IsActive { get; set; }
public OrganizationSettings Settings { get; set; } = new();
public List<FeatureFlag> Features { get; set; } = new();
public DateTime CreatedAt { get; set; }
public OrganizationStatisticsDto? Statistics { get; set; }
}
/// <summary>
/// DTO آمار سازمان
/// </summary>
public class OrganizationStatisticsDto
{
public int TotalUsers { get; set; }
public int ActiveUsers { get; set; }
public int TotalRoles { get; set; }
public DateTime? LastActivityDate { get; set; }
}
