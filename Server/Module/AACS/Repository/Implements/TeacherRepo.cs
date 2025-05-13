using System.Models;
using AACS.Repository.Interface;
using Core.Bus;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace AACS.Repository.Implements;
public class TeacherRepo : Repository<Teacher>, ITeacherRepo
{

    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;

    public TeacherRepo(ApplicationDbContext context,
        IMediatorHandler bus)
        : base(context)
    {
        _context = context;
        _bus = bus;
    }

    public async Task<Teacher> AddTeacherAsync(UserCreateReq req, string userId)
    {
        var teacher = new Teacher
        {
            UserId = userId,
            TeacherCode = req.TeacherCode,
            TeacherId = Guid.NewGuid().ToString(),
        };
        await _context.Teachers.AddAsync(teacher);
        return teacher;
    }

    public async Task<bool> DeleteTeacherAsync(string userId)
    {
        var teacher = await _context.Teachers.FirstAsync(e => e.UserId == userId);
        _context.Teachers.Remove(teacher);
        return true;
    }

    public async Task<Teacher> UpdateTeacherAsync(UserUpdateReq req, string userId)
    {
        var teacher = await _context.Teachers.FirstAsync(e => e.UserId == userId);
        teacher.TeacherCode = req.TeacherCode!;
        return teacher;
    }
}