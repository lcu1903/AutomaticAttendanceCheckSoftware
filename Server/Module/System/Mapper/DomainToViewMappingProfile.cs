using System.Models;
using AACS.Models;
using AutoMapper;
using DataAccess.Models;
namespace System.Mapper;

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
            .ForMember(dest => dest.Birthdate, opt => opt.MapFrom(src => src.BirthdayValue))
            .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.Student.Class.ClassId))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Student.Class.ClassName))
            .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.StudentCode))
            .ForMember(dest => dest.TeacherCode, opt => opt.MapFrom(src => src.Teacher.TeacherCode))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.StudentId))
            .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.Teacher.TeacherId))
            ;



    }
}