using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class SubjectScheduleStudentUpdateReq
{
    public string SubjectScheduleStudentId { get; set; } = null!;

    public string? SubjectScheduleId { get; set; }

    public string? StudentId { get; set; }

}
