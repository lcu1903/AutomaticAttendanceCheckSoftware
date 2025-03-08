using System.Security.Claims;

namespace System.Services;

public interface IJwtFactory
{
    Task<JwtToken> GenerateJwtToken(ClaimsIdentity claimsIdentity);
    bool IsValidToken(string token, out string userId);
}