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

        CreateMap<LoginReq, ApplicationUser>();


    }
}