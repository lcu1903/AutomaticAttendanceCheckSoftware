using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace DataAccess.Models;

public partial class SubjectScheduleStudent : EntityAudit
{
    public string SubjectScheduleStudentId { get; set; } = null!;

    public string? SubjectScheduleId { get; set; }

    public string? StudentId { get; set; }


    public virtual Student? Student { get; set; }

    public virtual SubjectSchedule? SubjectSchedule { get; set; }
}
