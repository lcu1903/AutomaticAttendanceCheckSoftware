using AACS.Repository.Interface;
using Core.Bus;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace AACS.Repository.Implements;

public class SubjectScheduleStudentRepo : Repository<SubjectScheduleStudent>, ISubjectScheduleStudentRepo
{

    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;

    public SubjectScheduleStudentRepo(ApplicationDbContext context,
        IMediatorHandler bus)
        : base(context)
    {
        _context = context;
        _bus = bus;
    }


}