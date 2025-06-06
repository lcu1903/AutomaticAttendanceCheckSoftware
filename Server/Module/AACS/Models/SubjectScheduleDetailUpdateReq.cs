﻿using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class SubjectScheduleDetailUpdateReq
{
    public string SubjectScheduleDetailId { get; set; } = null!;

    public string? SubjectScheduleId { get; set; }

    public DateTime ScheduleDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Note { get; set; }

}
