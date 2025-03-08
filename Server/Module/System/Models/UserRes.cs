namespace System.Models;

public class UserRes 
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } 
    public string? FullName { get; set; } 
}