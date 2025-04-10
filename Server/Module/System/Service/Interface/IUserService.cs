using System.Models;
using Core.Interfaces;

namespace System.Service.Interface;
public interface IUserService : IDomainService
{
    Task<IEnumerable<UserRes>> GetAllUsersAsync();
    Task<UserRes?> GetUserByIdAsync(string id);
    Task<UserRes?> CreateUserAsync(UserCreateReq req);
    Task<UserRes?> UpdateUserAsync(string userId, UserUpdateReq req);
    Task<bool> DeleteUserAsync(string userId);
    
}

