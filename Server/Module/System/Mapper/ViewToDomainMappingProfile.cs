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


    }
}