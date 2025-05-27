using Infrastructure.Models;

namespace DataAccess.Models;

public partial class Semester : EntityAudit
{
    public string SemesterId { get; set; } = null!;

    public string SemesterName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public virtual ICollection<SubjectSchedule> SubjectSchedules { get; set; } = new List<SubjectSchedule>();
}
