using DataAccess.Models;

namespace AACS.Models;
public class ClassUpdateReq
{
    public string ClassId { get; set; } = null!;

    public string ClassCode { get; set; } = null!;
    public string ClassName { get; set; } = null!;

    public DateTime? SchoolYearStart { get; set; }

    public DateTime? SchoolYearEnd { get; set; }

    public string? DepartmentId { get; set; }
    public string? HeadTeacherId { get; set; }
    public string? Room { get; set; }

}