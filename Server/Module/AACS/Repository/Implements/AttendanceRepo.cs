using AACS.Repository.Interface;
using Core.Bus;
using Core.Notifications;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;
using Humanizer;

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

    public bool AddAttendanceFromFaceRecognition(Attendance attendance, string? subjectScheduleDetailId)
    {
        var scheduleDetails = _context.SubjectScheduleDetails.First(e => e.SubjectScheduleDetailId == subjectScheduleDetailId);
        var localStartTime = scheduleDetails.StartTime.ToLocalTime();
        var localAttendanceTime = attendance.AttendanceTime.ToLocalTime();
        var timeSpan = new TimeSpan(localStartTime.Hour, localStartTime.Minute, localStartTime.Second);
        var attendanceTime = localAttendanceTime.TimeOfDay;
        if (attendanceTime > timeSpan && attendanceTime < timeSpan.Add(new TimeSpan(0, 15, 0)))
        {
            attendance.StatusId = "ON_TIME";
        }
        else
        {
            attendance.StatusId = "LATE";
            attendance.Note = $"Đi muộn {Math.Abs((attendanceTime - timeSpan).TotalMinutes).ToString("F2")} phút";
        }
        _context.Attendances.Add(attendance);
        return true;
    }
}