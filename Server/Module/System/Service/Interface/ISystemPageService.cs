using System.Models;
using Core.Interfaces;

namespace System.Service.Interface;
public interface ISystemPageService : IDomainService
{
    Task<List<SystemPageRes>> GetAllAsync(string? textSearch);
    Task<SystemPageRes?> GetByIdAsync(string id);
    Task<SystemPageRes?> AddAsync(SystemPageCreateReq req);
    Task<SystemPageRes?> UpdateAsync(string pageId, SystemPageUpdateReq req);
    Task<bool> DeleteAsync(string id);
    
}

