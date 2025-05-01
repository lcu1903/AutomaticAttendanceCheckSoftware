namespace System.Models;
public class SystemDepartmentUpdateReq
{
    public string DepartmentId { get; set; } = null!;

    public string? DepartmentName { get; set; }

    public string? DepartmentParentId { get; set; }
    public string? Description { get; set; }
}