using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace DataAccess.Models;

public partial class SubjectSchedule : EntityAudit
{
    public string SubjectScheduleId { get; set; } = null!;
    public string SubjectScheduleCode { get; set; } = null!;
    public string? SubjectId { get; set; }

    public string SemesterId { get; set; } = null!;

    public string? TeacherId { get; set; }

    public string? TeachingAssistant { get; set; }

    public string? RoomNumber { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Semester Semester { get; set; } = null!;

    public virtual Subject? Subject { get; set; }

    public virtual ICollection<SubjectScheduleStudent> SubjectScheduleStudents { get; set; } = new List<SubjectScheduleStudent>();

    public virtual Teacher? Teacher { get; set; }

    public virtual Teacher? TeachingAssistantNavigation { get; set; }
}
