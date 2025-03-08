namespace System.Models;
public class RegisterReq
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } 
    public string? FullName { get; set; }

}