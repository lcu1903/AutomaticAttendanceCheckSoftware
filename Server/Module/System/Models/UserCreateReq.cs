namespace System.Models;

public class UserCreateReq
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? FullName { get; set; } = null!;
}