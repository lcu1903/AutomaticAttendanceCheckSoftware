using DataAccess.Models;

namespace AACS.Models;
public class ClassRes
{
    public string ClassId { get; set; } = null!;

    public string ClassCode { get; set; } = null!;
    public string ClassName { get; set; } = null!;

    public string? SchoolYearStart { get; set; }

    public string? SchoolYearEnd { get; set; }

    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? HeadTeacherId { get; set; }
    public string? HeadTeacherName { get; set; }
    public string? Room { get; set; }

}