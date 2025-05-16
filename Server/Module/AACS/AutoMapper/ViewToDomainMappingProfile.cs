using System.Models;
using AACS.Models;
using AutoMapper;
using DataAccess.Models;

namespace AACS.Mapper;

public class ViewToDomainMappingProfile : Profile
{
    public ViewToDomainMappingProfile()
    {
        CreateMap<ClassCreateReq, Class>();
        CreateMap<ClassUpdateReq, Class>();

        CreateMap<TeacherCreateReq, Teacher>();
        CreateMap<TeacherUpdateReq, Teacher>();
        CreateMap<TeacherCreateReq, ApplicationUser>()
        .ForMember(dest => dest.BirthdayValue, opt => opt.MapFrom(src => src.Birthdate));

        CreateMap<StudentCreateReq, Student>();
        CreateMap<StudentUpdateReq, Student>();
        CreateMap<StudentCreateReq, ApplicationUser>()
        .ForMember(dest => dest.BirthdayValue, opt => opt.MapFrom(src => src.Birthdate))
        ;

        CreateMap<SemesterCreateReq, Semester>();
        CreateMap<SemesterUpdateReq, Semester>();

        CreateMap<SubjectCreateReq, Subject>();
        CreateMap<SubjectUpdateReq, Subject>();

        CreateMap<SubjectScheduleCreateReq, SubjectSchedule>();
        CreateMap<SubjectScheduleUpdateReq, SubjectSchedule>();

        CreateMap<SubjectScheduleDetailCreateReq, SubjectScheduleDetail>();



    }
}