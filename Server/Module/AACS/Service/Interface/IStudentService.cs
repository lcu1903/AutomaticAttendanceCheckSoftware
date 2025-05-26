using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;

public interface IStudentService : IDomainService
{
    Task<List<StudentRes>> GetAllAsync(string? textSearch, List<string>? departmentIds, List<string>? positionIds, List<string>? classIds);
    Task<StudentRes?> GetByIdAsync(string id);
    Task<StudentRes?> AddAsync(StudentCreateReq req);
    Task<StudentRes?> UpdateAsync(string teacherId, StudentUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
    Task<StudentRes?> GetByUserIdAsync(string userId);
}

