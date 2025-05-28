using AutoMapper;
using LendTech.Application.DTOs.Role;
using LendTech.Application.DTOs.Permission;
using LendTech.Database.Entities;
namespace LendTech.Application.Mappings;
/// <summary>
/// پروفایل نگاشت نقش
/// </summary>
public class RoleMappingProfile : Profile
{
public RoleMappingProfile()
{
// Role -> RoleDto
CreateMap<Role, RoleDto>()
.ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.UserRoles.Count))
.ForMember(dest => dest.PermissionGroups, opt => opt.MapFrom(src =>
src.RolePermissionGroups.Select(rpg => rpg.PermissionGroup)));
    // CreateRoleDto -> Role
    CreateMap<CreateRoleDto, Role>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
        .ForMember(dest => dest.IsSystemRole, opt => opt.MapFrom(src => false));

    // UpdateRoleDto -> Role
    CreateMap<UpdateRoleDto, Role>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
        .ForMember(dest => dest.IsSystemRole, opt => opt.Ignore());
}
}
