
namespace AACS.Models;

public partial class AttendanceHistoryStudentDetailRes
{
    public string? AttendanceId { get; set; }
    public DateTime? AttendanceTime { get; set; }
    public string? SubjectScheduleDetailId { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public string? StatusId { get; set; }

    public string? Note { get; set; }
    public string? ImageUrl { get; set; }

}
