using AACS.Repository.Interface;
using Core.Bus;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> DeleteDetailAsync(string detailId)
    {
        var detail = await _context.SubjectScheduleDetails.FindAsync(detailId);
        _context.SubjectScheduleDetails.Remove(detail);
        return true;
    }

    public IQueryable<SubjectScheduleDetail?> GetByDetailIdAsync(string id)
    {
        var result = _context.SubjectScheduleDetails.Where(e => e.SubjectScheduleId == id);
        return result;
    }
}