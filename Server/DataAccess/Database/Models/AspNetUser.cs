using System.Security.Claims;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DataAccess.Models;

public class AspNetUser : IUser
{
    private readonly IHttpContextAccessor _accessor;

    public AspNetUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    // public string Name => _accessor.HttpContext.User.FindFirst(e => e.Type == ClaimTypes.NameIdentifier).Value;
    public string Name
    {
        get
        {
            var user = _accessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return "guest"; // Return a default or external identifier for non-authenticated users
            }
            return user.FindFirst(e => e.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext.User.Identity.IsAuthenticated;
    }

    public IEnumerable<Claim> GetClaimsIdentity()
    {
        return _accessor.HttpContext.User.Claims;
    }
}