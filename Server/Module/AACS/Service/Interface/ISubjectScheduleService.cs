using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;
public interface ISubjectScheduleService : IDomainService
{
    Task<List<SubjectScheduleRes>> GetAllAsync(string? textSearch);
    Task<SubjectScheduleRes?> GetByIdAsync(string id);
    Task<SubjectScheduleRes?> AddAsync(SubjectScheduleCreateReq req);
    Task<SubjectScheduleRes?> UpdateAsync(string subjectScheduleId, SubjectScheduleUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

