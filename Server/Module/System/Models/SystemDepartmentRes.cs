namespace System.Models;
public class SystemDepartmentRes
{
    public string DepartmentId { get; set; } = null!;

    public string? DepartmentName { get; set; }

    public string? DepartmentParentId { get; set; }
    public string? DepartmentParentName { get; set; }
    public string? Description { get; set; }
}