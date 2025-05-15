using System.Models;
using AACS.Models;
using AutoMapper;
using DataAccess.Models;
namespace AACS.Mapper;

public class DomainToViewMappingProfile : Profile
{
    public DomainToViewMappingProfile()
    {
        CreateMap<Class, ClassRes>()
            .ForMember(dest => dest.HeadTeacherName, opt => opt.MapFrom(src => src.HeadTeacher.FullName))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
        ;
        CreateMap<Teacher, UserRes>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.User.Department.DepartmentName))
            .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.User.Position.PositionName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
        ;
        CreateMap<Student, UserRes>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.User.Department.DepartmentName))
            .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.User.Position.PositionName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            ;
        CreateMap<Student, StudentRes>()
            .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))

            ;
        CreateMap<Teacher, TeacherRes>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            ;
        CreateMap<Semester, SemesterRes>();

        CreateMap<Subject, SubjectRes>()
            ;
        CreateMap<SubjectSchedule, SubjectScheduleRes>()
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester))
            .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.Teacher))
            .ForMember(dest => dest.TeachingAssistantNavigation, opt => opt.MapFrom(src => src.TeachingAssistantNavigation));
            
    }
}