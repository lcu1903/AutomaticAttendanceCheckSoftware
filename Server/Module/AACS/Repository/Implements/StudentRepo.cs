using System.Models;
using AACS.Repository.Interface;
using Core.Bus;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace AACS.Repository.Implements;
public class StudentRepo : Repository<Student>, IStudentRepo
{

    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;

    public StudentRepo(ApplicationDbContext context,
        IMediatorHandler bus)
        : base(context)
    {
        _context = context;
        _bus = bus;
    }

    public async Task<Student> AddStudentAsync(UserCreateReq req, string userId)
    {
        var student = new Student
        {
            UserId = userId,
            ClassId = req.ClassId,
            StudentCode = req.StudentCode,
            StudentId = Guid.NewGuid().ToString(),
        };
        await _context.Students.AddAsync(student);
        return student;
    }

    public async Task<bool> DeleteStudentAsync(string userId)
    {
        var student = await _context.Students.FirstAsync(e => e.UserId == userId);
        _context.Students.Remove(student);
        return true;
    }

    public async Task<Student> UpdateStudentAsync(UserUpdateReq req, string userId)
    {
        var student = await _context.Students.FirstAsync(e => e.UserId == userId);
        student.ClassId = req.ClassId;
        student.StudentCode = req.StudentCode!;
        return student;
    }
}