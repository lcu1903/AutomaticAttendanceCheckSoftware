using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class SubjectScheduleStudentRes
{
    public string SubjectScheduleStudentId { get; set; } = null!;

    public string? SubjectScheduleId { get; set; }

    public string? StudentId { get; set; }
    public string? ClassRoom { get; set; }
    public string? TeacherName { get; set; }


    public string SubjectScheduleCode { get; set; } = null!;
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool? IsCheckable { get; set; }

    // public virtual SubjectScheduleRes? SubjectSchedule { get; set; }
}
