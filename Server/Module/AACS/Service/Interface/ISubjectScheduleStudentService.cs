using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;

public interface ISubjectScheduleStudentService : IDomainService
{
    Task<List<SubjectScheduleStudentRes>> GetAllAsync(string? textSearch, List<string>? studentIds);
    Task<SubjectScheduleStudentRes?> GetByIdAsync(string id);
    Task<List<SubjectScheduleStudentRes>> AddAsync(List<SubjectScheduleStudentCreateReq> req);
    Task<List<SubjectScheduleStudentRes>> GetByStudentIdAsync(string studentId);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

