using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace DataAccess.Models;

public partial class SubjectScheduleDetail : EntityAudit
{
    public string SubjectScheduleDetailId { get; set; } = null!;

    public string? SubjectScheduleId { get; set; }

    public DateTime ScheduleDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Note { get; set; }
    public virtual SubjectSchedule? SubjectSchedule { get; set; } = null!;

}
