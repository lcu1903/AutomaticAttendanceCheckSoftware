using System.Models;

namespace AACS.Models;
public partial class SubjectCreateReq
{

    public string SubjectCode { get; set; } = null!;

    public string? SubjectName { get; set; }

    public int SubjectCredits { get; set; }

    // public virtual ICollection<SubjectSchedule> SubjectSchedules { get; set; } = new List<SubjectSchedule>();

}
