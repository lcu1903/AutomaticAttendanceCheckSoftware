using System.Models;
using AutoMapper;
using DataAccess.Models;

namespace Lottery.Mapper;

public class ViewToDomainMappingProfile : Profile
{
    public ViewToDomainMappingProfile()
    {
        CreateMap<SystemPageCreateReq, SystemPage>();
        CreateMap<SystemPageUpdateReq, SystemPage>();

        CreateMap<SystemDepartmentCreateReq, SystemDepartment>();
        CreateMap<SystemDepartmentUpdateReq, SystemDepartment>();

        CreateMap<SystemPositionCreateReq, SystemPosition>();
        CreateMap<SystemPositionUpdateReq, SystemPosition>();

        CreateMap<LoginReq, ApplicationUser>();
        CreateMap<UserCreateReq, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.BirthdayValue, opt => opt.MapFrom(src => src.Birthdate));


    }
}