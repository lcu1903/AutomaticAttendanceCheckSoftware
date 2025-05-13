using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;
public interface ITeacherService : IDomainService
{
    Task<List<TeacherRes>> GetAllAsync(string? textSearch, List<string>? departmentIds, List<string>? positionIds);
    Task<TeacherRes?> GetByIdAsync(string id);
    Task<TeacherRes?> AddAsync(TeacherCreateReq req);
    Task<TeacherRes?> UpdateAsync(string teacherId, TeacherUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

