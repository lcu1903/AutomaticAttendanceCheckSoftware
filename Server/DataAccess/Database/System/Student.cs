using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace DataAccess.Models;

public partial class Student : EntityAudit
{
    public string StudentId { get; set; } = null!;

    public string StudentCode { get; set; } = null!;

    public string? ClassId { get; set; }

    public string? UserId { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<SubjectScheduleStudent> SubjectScheduleStudents { get; set; } = new List<SubjectScheduleStudent>();
}
