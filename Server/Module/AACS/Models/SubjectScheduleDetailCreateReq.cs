
namespace AACS.Models;

public partial class SubjectScheduleDetailCreateReq
{

    public string? SubjectScheduleId { get; set; }

    public DateTime ScheduleDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Note { get; set; }

}
