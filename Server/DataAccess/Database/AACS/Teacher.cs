using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace DataAccess.Models;

public partial class Teacher : EntityAudit
{
    public string TeacherId { get; set; } = null!;

    public string TeacherCode { get; set; } = null!;

    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }

    public virtual ICollection<SubjectSchedule> SubjectScheduleTeachers { get; set; } = new List<SubjectSchedule>();

    public virtual ICollection<SubjectSchedule> SubjectScheduleTeachingAssistant { get; set; } = new List<SubjectSchedule>();
}
