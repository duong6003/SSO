using AutoMapper;
using Infrastructure.Common.Validations;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;
using System.Text.RegularExpressions;

namespace Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // user
        CreateMap<UserSignUpRequest, User>()
            .ForMember(dest => dest.Avatar, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.MapFrom((src, dest) => dest.Password = BCrypt.Net.BCrypt.HashPassword(src.Password!)));
        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Avatar, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.UserPermissions, opt => opt.Ignore())
            .ForAllMembers(x => x.Condition((src, dest, obj) => obj is not null || !string.IsNullOrWhiteSpace((string?)obj)));
        CreateMap<UpdateProfileRequest, User>()
            .ForMember(dest => dest.Avatar, opt => opt.Ignore())
            .ForMember(dest => dest.UserPermissions, opt => opt.Ignore())
            .ForAllMembers(x => x.Condition((src, dest, obj) => obj is not null || !string.IsNullOrWhiteSpace((string?)obj)));
        CreateMap<UpdateUserPermissionRequest, UserPermission>();
        CreateMap<CreateUserPermissionRequest, UserPermission>();
        // role
        CreateMap<CreateRoleRequest, Role>();
        CreateMap<UpdateRoleRequest, Role>().ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
            .ForAllMembers(x => x.Condition((src, dest, obj) => obj is not null || !string.IsNullOrWhiteSpace((string?)obj))); ;

        CreateMap<UpdateRolePermissionRequest, RolePermission>();
        CreateMap<CreateRolePermissionRequest, RolePermission>();

        CreateMap<CreatePermissionRequest, Permission>();
        CreateMap<UpdatePermissionRequest, Permission>()
            .ForAllMembers(x => x.Condition((src, dest, obj) => obj is not null || !string.IsNullOrWhiteSpace((string?)obj)));
    }
}