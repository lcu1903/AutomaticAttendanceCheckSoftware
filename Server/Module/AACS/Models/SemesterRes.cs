using Infrastructure.Models;

namespace AACS.Models;

public partial class SemesterRes
{
    public string SemesterId { get; set; } = null!;

    public string SemesterName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    // public virtual ICollection<SubjectSchedule> SubjectSchedules { get; set; } = new List<SubjectSchedule>();
}
