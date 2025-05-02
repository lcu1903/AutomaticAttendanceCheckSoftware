namespace System.Models;

public class UserRes
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionId { get; set; }
    public string? PositionName { get; set; }
    public DateTime? Birthdate { get; set; }
}