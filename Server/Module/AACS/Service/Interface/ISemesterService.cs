using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;
public interface ISemesterService : IDomainService
{
    Task<List<SemesterRes>> GetAllAsync(string? textSearch);
    Task<SemesterRes?> GetByIdAsync(string id);
    Task<SemesterRes?> AddAsync(SemesterCreateReq req);
    Task<SemesterRes?> UpdateAsync(string semesterId, SemesterUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

