using System.Models;
using AutoMapper;
using DataAccess.Models;
namespace Lottery.Mapper;

public class DomainToViewMappingProfile : Profile
{
    public DomainToViewMappingProfile()
    {
        CreateMap<SystemPage, SystemPageRes>();
        CreateMap<UserCreateReq, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => false));


    }
}