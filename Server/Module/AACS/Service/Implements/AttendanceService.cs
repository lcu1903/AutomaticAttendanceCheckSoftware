using Infrastructure.DomainService;
using Core.Bus;
using DataAccess.Models;
using MediatR;
using Core.Notifications;
using Core.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AACS.Service.Interface;
using AACS.Repository.Interface;
using AACS.Models;

namespace AACS.Service.Implements;

public class AttendanceService : DomainService, IAttendanceService
{
    private readonly IMediatorHandler _bus;
    private readonly IAttendanceRepo _attendanceRepo;
    private readonly IMapper _mapper;
    private readonly ISubjectScheduleStudentRepo _subjectScheduleStudentRepo;
    public AttendanceService(
        IAttendanceRepo attendanceRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        ISubjectScheduleStudentRepo subjectScheduleStudentRepo,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _attendanceRepo = attendanceRepo;
        _subjectScheduleStudentRepo = subjectScheduleStudentRepo;
    }

    public async Task<AttendanceRes?> AddAsync(AttendanceCreateReq req)
    {
        var attendance = _mapper.Map<Attendance>(req);
        attendance.AttendanceId = Guid.NewGuid().ToString();
        _attendanceRepo.Add(attendance);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(attendance.AttendanceId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var attendance = await _attendanceRepo.GetAll().FirstOrDefaultAsync(e => e.AttendanceId == id);
        if (attendance is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceNotFound"));
            return false;
        }
        _attendanceRepo.Remove(attendance.AttendanceId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceDeleteFailed"));
            return false;
        }
    }

    public async Task<List<AttendanceRes>> GetAllAsync(string? textSearch, List<string>? subjectIds, DateTime? fromDate, DateTime? toDate, int? pageIndex, int? pageSize, string? userId)
    {
        var attendances = _attendanceRepo.GetAll();
        // if (!string.IsNullOrEmpty(textSearch))
        // {
        //     attendances = attendances.Where(e => e.AttendanceName.ToLower().Contains(textSearch.ToLower()));
        // }
        if (!string.IsNullOrEmpty(userId))
        {
            attendances = attendances.Where(e => e.UserId == userId);
        }
        if (subjectIds != null && subjectIds.Count > 0)
        {
            // attendances = attendances.Where(e => subjectIds.Contains(e.SubjectScheduleDetail.SubjectSchedule.SubjectId));
        }
        if (fromDate.HasValue && toDate.HasValue)
        {
            attendances = attendances.Where(e => fromDate.Value <= e.AttendanceTime && e.AttendanceTime <= toDate.Value);
        }
        if (pageIndex != null && pageSize != null)
        {
            attendances = attendances
            .Skip(pageIndex.Value * pageSize.Value)
            .Take(pageSize.Value);
        }
        return await attendances.OrderByDescending(e => e.AttendanceTime).ProjectTo<AttendanceRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<AttendanceRes?> GetByIdAsync(string id)
    {
        var attendance = _attendanceRepo.GetAll().Where(e => e.AttendanceId == id);
        if (attendance is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceNotFound"));
            return null;
        }
        return await attendance.Take(1).ProjectTo<AttendanceRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<AttendanceRes?> UpdateAsync(string classId, AttendanceUpdateReq req)
    {
        var attendance = await _attendanceRepo.GetAll().FirstOrDefaultAsync(e => e.AttendanceId == classId);
        if (attendance is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceNotFound"));
            return null;
        }
        attendance.AttendanceId = req.AttendanceId;
        attendance.AttendanceTime = req.AttendanceTime;
        attendance.UserId = req.UserId;
        attendance.SubjectScheduleDetailId = req.SubjectScheduleDetailId;
        attendance.StatusId = req.StatusId;
        attendance.Note = req.Note;
        attendance.ImageUrl = req.ImageUrl;


        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(attendance.AttendanceId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var classIds = await _attendanceRepo.GetAll().Where(e => ids.Contains(e.AttendanceId)).Select(e => e.AttendanceId).ToListAsync();
        if (classIds is null || classIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceNotFound"));
            return false;
        }
        foreach (var id in classIds)
        {
            _attendanceRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.attendanceDeleteFailed"));
            return false;
        }
    }

    public async Task<List<AttendanceHistoryStudentRes?>> GetAttendanceHistoryByUserIdAsync(string userId, string? subjectId, string? semesterId)
    {
        // Lấy danh sách điểm danh của user
        var attendances = _attendanceRepo.GetAll()
        .OrderByDescending(e => e.AttendanceTime)
        .Where(e => e.UserId == userId);

        // Lấy danh sách lịch trình môn học
        var subjectSchedules = _subjectScheduleStudentRepo.GetAll()
            .Where(e => e.Student.UserId == userId && (subjectId == null || e.SubjectSchedule.SubjectId == subjectId) && (semesterId == null || e.SubjectSchedule.SemesterId == semesterId))
            .Select(e => e.SubjectSchedule);
        // Tạo danh sách kết quả
        var attendanceHistory = await subjectSchedules.Select(subjectSchedule => new AttendanceHistoryStudentRes
        {
            SubjectCode = subjectSchedule.Subject.SubjectCode,
            SubjectName = subjectSchedule.Subject.SubjectName,
            RoomNumber = subjectSchedule.RoomNumber,
            TeacherCode = subjectSchedule.Teacher.TeacherCode,
            TeacherName = subjectSchedule.Teacher.User.FullName,
            UserId = userId,
            StartDate = subjectSchedule.StartDate,
            EndDate = subjectSchedule.EndDate,
            AttendanceDetails = subjectSchedule.SubjectScheduleDetails
                .Select(detail => new AttendanceHistoryStudentDetailRes
                {
                    ScheduleDate = detail.ScheduleDate,
                    StartTime = detail.StartTime,
                    EndTime = detail.EndTime,
                    SubjectScheduleDetailId = detail.SubjectScheduleDetailId,
                    AttendanceId = attendances.FirstOrDefault(a => a.SubjectScheduleDetailId == detail.SubjectScheduleDetailId).AttendanceId,
                    AttendanceTime = attendances.FirstOrDefault(a => a.SubjectScheduleDetailId == detail.SubjectScheduleDetailId).AttendanceTime,
                    StatusId = attendances.FirstOrDefault(a => a.SubjectScheduleDetailId == detail.SubjectScheduleDetailId).StatusId,
                    Note = attendances.FirstOrDefault(a => a.SubjectScheduleDetailId == detail.SubjectScheduleDetailId).Note,
                    ImageUrl = attendances.FirstOrDefault(a => a.SubjectScheduleDetailId == detail.SubjectScheduleDetailId).ImageUrl
                }).OrderBy(e => e.ScheduleDate).ToList()
        }).ToListAsync();

        return attendanceHistory;
    }
}
