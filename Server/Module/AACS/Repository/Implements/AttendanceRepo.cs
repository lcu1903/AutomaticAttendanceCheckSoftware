using AACS.Repository.Interface;
using Core.Bus;
using Core.Notifications;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;

namespace AACS.Repository.Implements;

public class AttendanceRepo : Repository<Attendance>, IAttendanceRepo
{

    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;

    public AttendanceRepo(ApplicationDbContext context,
        IMediatorHandler bus)
        : base(context)
    {
        _context = context;
        _bus = bus;
    }

    public bool AddAttendanceFromFaceRecognition(Attendance attendance)
    {
        _context.Attendances.Add(attendance);
        return true;
    }
}