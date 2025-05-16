using System;
using System.Collections.Generic;
using Infrastructure.Models;

namespace AACS.Models;

public partial class SubjectScheduleDetailChangeScheduleReq
{

    public string SubjectScheduleId { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public List<DateTime> ListScheduleDate { get; set; } = new List<DateTime>();


}
