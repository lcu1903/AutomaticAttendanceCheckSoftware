using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class SubjectScheduleStudentRes
{
    public string SubjectScheduleStudentId { get; set; } = null!;

    public string? SubjectScheduleId { get; set; }

    public string? StudentId { get; set; }


    // public virtual StudentRes? Student { get; set; }

    // public virtual SubjectScheduleRes? SubjectSchedule { get; set; }
}
