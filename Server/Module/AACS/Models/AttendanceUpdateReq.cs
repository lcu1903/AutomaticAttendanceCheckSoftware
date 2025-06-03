
namespace AACS.Models;

public partial class AttendanceUpdateReq
{
    public string AttendanceId { get; set; } = null!;
    public DateTime AttendanceTime { get; set; }

    public string? UserId { get; set; }

    public string? SubjectScheduleDetailId { get; set; }

    public string? StatusId { get; set; }

    public string? Note { get; set; }
    public string? ImageUrl { get; set; }
}
