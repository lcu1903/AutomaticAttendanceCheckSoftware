using AACS.Repository.Interface;
using Core.Bus;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;

namespace AACS.Repository.Implements;
public class SubjectScheduleRepo : Repository<SubjectSchedule>, ISubjectScheduleRepo
{

    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;

    public SubjectScheduleRepo(ApplicationDbContext context,
        IMediatorHandler bus)
        : base(context)
    {
        _context = context;
        _bus = bus;
    }

}