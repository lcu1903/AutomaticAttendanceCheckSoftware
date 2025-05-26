using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class SubjectScheduleStudentCreateReq
{
    public string? SubjectScheduleId { get; set; }

    public string? StudentId { get; set; }

}
