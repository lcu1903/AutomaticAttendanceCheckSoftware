
namespace AACS.Models;

public partial class AttendanceRes
{
    public string AttendanceId { get; set; } = null!;

    public DateTime AttendanceTime { get; set; }

    public string? UserId { get; set; }

    public string? SubjectScheduleDetailId { get; set; }

    public string? StatusId { get; set; }

    public string? Note { get; set; }
    public string? ImageUrl { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public string? TeacherCode { get; set; }
    public string? TeacherName { get; set; }
    public string? RoomNumber { get; set; }

}
