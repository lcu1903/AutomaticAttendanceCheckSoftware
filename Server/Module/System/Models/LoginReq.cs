namespace System.Models;

public class LoginReq
{
    //Username == Email
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}