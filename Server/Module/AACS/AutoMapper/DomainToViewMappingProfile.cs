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
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
            .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode))
            .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => src.Semester.SemesterName))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.User.FullName))
            .ForMember(dest => dest.TeacherCode, opt => opt.MapFrom(src => src.Teacher.TeacherCode))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName))
            .ForMember(dest => dest.ClassCode, opt => opt.MapFrom(src => src.Class.ClassCode))
            .ForMember(dest => dest.TeachingAssistantName, opt => opt.MapFrom(src => src.TeachingAssistantNavigation.User.FullName))
            .ForMember(dest => dest.TeachingAssistantCode, opt => opt.MapFrom(src => src.TeachingAssistantNavigation.TeacherCode))
            .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.SubjectScheduleStudents.Select(x => x.Student)))

            .ForMember(dest => dest.SubjectScheduleDetails, opt => opt.MapFrom(src => src.SubjectScheduleDetails.OrderBy(x => x.ScheduleDate)))
            ;
        CreateMap<SubjectScheduleDetail, SubjectScheduleDetailRes>();
        CreateMap<SubjectScheduleStudent, SubjectScheduleStudentRes>()
            ;

    }
}