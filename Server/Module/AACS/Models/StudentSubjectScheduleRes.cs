using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class StudentSubjectScheduleRes
{

    public string? StudentId { get; set; }
    public string StudentCode { get; set; } = null!;
    public string? FullName { get; set; }
    public string? ClassId { get; set; }

    public virtual List<SubjectScheduleRes?> SubjectSchedules { get; set; } = new List<SubjectScheduleRes?>();
}
