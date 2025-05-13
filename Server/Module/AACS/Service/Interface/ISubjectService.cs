using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;
public interface ISubjectService : IDomainService
{
    Task<List<SubjectRes>> GetAllAsync(string? textSearch);
    Task<SubjectRes?> GetByIdAsync(string id);
    Task<SubjectRes?> AddAsync(SubjectCreateReq req);
    Task<SubjectRes?> UpdateAsync(string subjectId, SubjectUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

