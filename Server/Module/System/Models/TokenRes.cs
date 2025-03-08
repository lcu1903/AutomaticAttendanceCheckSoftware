namespace System.Models;

public class TokenRes
{
    public string? AccessToken { get; set; } 
    public string? RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
}