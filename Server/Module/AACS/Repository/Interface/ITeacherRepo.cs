
using System.Models;
using Core.Interfaces;
using DataAccess.Models;

namespace AACS.Repository.Interface;

public interface ITeacherRepo : IRepository<Teacher>
{
    public Task<Teacher> AddTeacherAsync(UserCreateReq req, string userId);
    Task<Teacher> UpdateTeacherAsync(UserUpdateReq req, string userId);
    Task<bool> DeleteTeacherAsync(string userId);

}