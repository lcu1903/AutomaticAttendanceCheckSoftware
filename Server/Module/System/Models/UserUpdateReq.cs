namespace System.Models;
public class UserUpdateReq
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FullName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public string? StudentCode { get; set; }
    public string? TeacherCode { get; set; }
    public string? ClassId { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? Birthdate { get; set; }

}