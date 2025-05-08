using Infrastructure.Models;

namespace DataAccess.Models;

public partial class Class : EntityAudit
{
    public string ClassId { get; set; } = null!;

    public string ClassCode { get; set; } = null!;
    public string ClassName { get; set; } = null!;

    public string? SchoolYearStart { get; set; }

    public string? SchoolYearEnd { get; set; }

    public string? DepartmentId { get; set; }
    public string? HeadTeacherId { get; set; }
    public string? Room { get; set; }

    public virtual SystemDepartment? Department { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ApplicationUser? HeadTeacher { get; set; }
}
