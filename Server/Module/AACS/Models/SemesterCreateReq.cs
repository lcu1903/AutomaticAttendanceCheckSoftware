using Infrastructure.Models;

namespace AACS.Models;

public partial class SemesterCreateReq
{

    public string SemesterName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
