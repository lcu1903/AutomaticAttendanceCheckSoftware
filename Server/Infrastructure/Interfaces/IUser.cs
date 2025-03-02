using System.Security.Claims;

namespace Core.Interfaces;

public interface IUser
{
    string Name { get; }
    bool IsAuthenticated();
    IEnumerable<Claim> GetClaimsIdentity();
}