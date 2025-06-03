
using AACS.Models;
using Core.Interfaces;

namespace AACS.Service.Interface;

public interface IAttendanceService : IDomainService
{
    Task<List<AttendanceRes>> GetAllAsync(string? textSearch, List<string>? subjectIds, DateTime? fromDate, DateTime? toDate, int? pageIndex, int? pageSize, string? userId);
    Task<AttendanceRes?> GetByIdAsync(string id);
    Task<AttendanceRes?> AddAsync(AttendanceCreateReq req);
    Task<AttendanceRes?> UpdateAsync(string attendanceId, AttendanceUpdateReq req);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteRangeAsync(List<string> ids);
    Task<List<AttendanceHistoryStudentRes?>> GetAttendanceHistoryByUserIdAsync(string userId, string? subjectId, string? semesterId);
}

