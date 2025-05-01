using System.Models;
using Core.Interfaces;

namespace System.Service.Interface;
public interface ISystemDepartmentService : IDomainService
{
    Task<List<SystemDepartmentRes>> GetAllAsync(string? textSearch);
    Task<SystemDepartmentRes?> GetByIdAsync(string id);
    Task<SystemDepartmentRes?> AddAsync(SystemDepartmentCreateReq req);
    Task<SystemDepartmentRes?> UpdateAsync(string departmentId, SystemDepartmentUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

