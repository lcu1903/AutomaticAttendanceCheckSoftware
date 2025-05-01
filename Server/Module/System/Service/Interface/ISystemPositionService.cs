using System.Models;
using Core.Interfaces;

namespace System.Service.Interface;
public interface ISystemPositionService : IDomainService
{
    Task<List<SystemPositionRes>> GetAllAsync(string? textSearch);
    Task<SystemPositionRes?> GetByIdAsync(string id);
    Task<SystemPositionRes?> AddAsync(SystemPositionCreateReq req);
    Task<SystemPositionRes?> UpdateAsync(string positionId, SystemPositionUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
}

