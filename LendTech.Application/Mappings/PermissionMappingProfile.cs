using AutoMapper;
using LendTech.Application.DTOs.Permission;
using LendTech.Database.Entities;
namespace LendTech.Application.Mappings;
/// <summary>
/// پروفایل نگاشت دسترسی
/// </summary>
public class PermissionMappingProfile : Profile
{
public PermissionMappingProfile()
{
// Permission -> PermissionDto
CreateMap<Permission, PermissionDto>()
.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.PermissionGroup.Name));
    // PermissionGroup -> PermissionGroupDto
    CreateMap<PermissionGroup, PermissionGroupDto>();
}
}
