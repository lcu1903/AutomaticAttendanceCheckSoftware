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

}