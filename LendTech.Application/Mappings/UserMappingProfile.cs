using AutoMapper;
using LendTech.Application.DTOs.User;
using LendTech.Database.Entities;
namespace LendTech.Application.Mappings;
/// <summary>
/// پروفایل نگاشت کاربر
/// </summary>
public class UserMappingProfile : Profile
{
public UserMappingProfile()
{
// User -> UserDto
CreateMap<User, UserDto>()
.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));
    // User -> UserListDto
    CreateMap<User, UserListDto>()
        .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
        .ForMember(dest => dest.RoleCount, opt => opt.MapFrom(src => src.UserRoles.Count))
        .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore()); // TODO: از جدول لاگ‌ها بخوانیم

    // CreateUserDto -> User
    CreateMap<CreateUserDto, User>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => false))
        .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0));

    // UpdateUserDto -> User
    CreateMap<UpdateUserDto, User>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
        .ForMember(dest => dest.Username, opt => opt.Ignore())
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
}
}
