
namespace AACS.Models;

public partial class AttendanceHistoryStudentRes
{
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public string? TeacherCode { get; set; }
    public string? TeacherName { get; set; }
    public string? RoomNumber { get; set; }
    public string? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<AttendanceHistoryStudentDetailRes> AttendanceDetails { get; set; } = new List<AttendanceHistoryStudentDetailRes>();

}
