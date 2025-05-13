using System.Models;
using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;
public interface IClassService : IDomainService
{
    Task<List<ClassRes>> GetAllAsync(string? textSearch);
    Task<ClassRes?> GetByIdAsync(string id);
    Task<ClassRes?> AddAsync(ClassCreateReq req);
    Task<ClassRes?> UpdateAsync(string departmentId, ClassUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

