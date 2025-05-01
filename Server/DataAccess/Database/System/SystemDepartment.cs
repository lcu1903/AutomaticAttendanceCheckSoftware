using Infrastructure.Models;

namespace DataAccess.Models;
public class SystemDepartment : EntityAudit
{
    public string DepartmentId { get; set; } = null!;

    public string? DepartmentName { get; set; }

    public string? DepartmentParentId { get; set; }
    public string? Description { get; set; }
    public virtual SystemDepartment? DepartmentParent { get; set; }
}
