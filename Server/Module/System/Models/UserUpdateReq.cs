namespace System.Models;
public class UserUpdateReq 
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? FullName { get; set; } = null!;
}