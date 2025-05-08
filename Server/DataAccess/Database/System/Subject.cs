using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace DataAccess.Models;

public partial class Subject : EntityAudit
{
    public string SubjectId { get; set; } = null!;

    public string SubjectCode { get; set; } = null!;

    public string? SubjectName { get; set; }

    public int SubjectCredits { get; set; }

    public virtual ICollection<SubjectSchedule> SubjectSchedules { get; set; } = new List<SubjectSchedule>();
}
