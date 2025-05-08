
using System.Models;
using Core.Interfaces;
using DataAccess.Models;

namespace System.Repository.Interface;

public interface IStudentRepo : IRepository<Student>
{
    public Task<Student> AddStudentAsync(UserCreateReq req, string userId);
    Task<Student> UpdateStudentAsync(UserUpdateReq req, string userId);
    Task<bool> DeleteStudentAsync(string userId);
}