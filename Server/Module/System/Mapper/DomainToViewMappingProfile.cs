using System.Models;
using AutoMapper;
using DataAccess.Models;
namespace Lottery.Mapper;

public class DomainToViewMappingProfile : Profile
{
    public DomainToViewMappingProfile()
    {
        CreateMap<SystemPage, SystemPageRes>();
        CreateMap<SystemDepartment, SystemDepartmentRes>()
            .ForMember(dto => dto.DepartmentParentName, db => db.MapFrom(src => src.DepartmentParent.DepartmentName));
        CreateMap<SystemPosition, SystemPositionRes>()
            .ForMember(dto => dto.PositionParentName, db => db.MapFrom(src => src.PositionParent.PositionName));
        CreateMap<ApplicationUser, UserRes>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
            .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position.PositionName))
            ;



    }
}