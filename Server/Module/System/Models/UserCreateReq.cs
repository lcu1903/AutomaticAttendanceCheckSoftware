namespace System.Models;

public class UserCreateReq
{
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FullName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public DateTime? Birthdate { get; set; }
}