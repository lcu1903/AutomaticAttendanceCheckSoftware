
namespace AACS.Models;

public partial class SubjectScheduleRes
{
    public string SubjectScheduleId { get; set; } = null!;
    public string SubjectScheduleCode { get; set; } = null!;

    public string? SubjectId { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public string SemesterId { get; set; } = null!;
    public string? SemesterName { get; set; }

    public string? TeacherId { get; set; }
    public string? TeacherCode { get; set; }
    public string? TeacherName { get; set; }
    public string? ClassId { get; set; }

    public string? ClassCode { get; set; }
    public string? ClassName { get; set; }
    public string? TeachingAssistant { get; set; }
    public string? TeachingAssistantCode { get; set; }
    public string? TeachingAssistantName { get; set; }

    public string? RoomNumber { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Note { get; set; }
    public List<StudentRes> Students { get; set; } = new List<StudentRes>();

    // public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    // public virtual SemesterRes Semester { get; set; } = null!;

    // public virtual SubjectRes? Subject { get; set; }

    // public virtual ICollection<SubjectScheduleStudentRes> SubjectScheduleStudents { get; set; } = new List<SubjectScheduleStudentRes>();

    // public virtual TeacherRes? Teacher { get; set; }

    // public virtual TeacherRes? TeachingAssistantNavigation { get; set; }
    // public virtual ClassRes? Class { get; set; }
    public virtual ICollection<SubjectScheduleDetailRes> SubjectScheduleDetails { get; set; } = new List<SubjectScheduleDetailRes>();
}
