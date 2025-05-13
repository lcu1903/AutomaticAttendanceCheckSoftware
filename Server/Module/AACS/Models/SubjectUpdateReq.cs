using System.Models;

namespace AACS.Models;
public partial class SubjectUpdateReq
{
    public string SubjectId { get; set; } = null!;

    public string SubjectCode { get; set; } = null!;

    public string? SubjectName { get; set; }

    public int SubjectCredits { get; set; }

    // public virtual ICollection<SubjectSchedule> SubjectSchedules { get; set; } = new List<SubjectSchedule>();

}
