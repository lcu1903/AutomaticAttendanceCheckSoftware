using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class StudentSubjectScheduleRegisterReq
{

    public string? StudentId { get; set; }
    public List<string> SubjectScheduleIds { get; set; } = new List<string>();

}
