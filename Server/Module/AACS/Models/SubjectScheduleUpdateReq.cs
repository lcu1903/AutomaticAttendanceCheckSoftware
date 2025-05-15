
namespace AACS.Models;
public partial class SubjectScheduleUpdateReq
{
    public string SubjectScheduleId { get; set; } = null!;
    public string SubjectScheduleCode { get; set; } = null!;

    public string? SubjectId { get; set; }

    public string SemesterId { get; set; } = null!;

    public string? TeacherId { get; set; }

    public string? TeachingAssistant { get; set; }

    public string? RoomNumber { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Note { get; set; }
    public string? ClassId { get; set; }

    // public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    // public virtual SemesterRes Semester { get; set; } = null!;

    // public virtual SubjectRes? Subject { get; set; }

    // // public virtual ICollection<SubjectScheduleStudent> SubjectScheduleStudents { get; set; } = new List<SubjectScheduleStudent>();

    // public virtual TeacherRes? Teacher { get; set; }

    // public virtual TeacherRes? TeachingAssistantNavigation { get; set; }
}
