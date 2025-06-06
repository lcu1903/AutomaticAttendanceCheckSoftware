﻿using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace DataAccess.Models;

public partial class Attendance : EntityAudit
{
    public string AttendanceId { get; set; } = null!;

    public DateTime AttendanceTime { get; set; }

    public string? UserId { get; set; }

    public string? SubjectScheduleDetailId { get; set; }

    public string? StatusId { get; set; }

    public string? Note { get; set; }
    public string? ImageUrl { get; set; }

    public virtual SubjectScheduleDetail? SubjectScheduleDetail { get; set; }
}
