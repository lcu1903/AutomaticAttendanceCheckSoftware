using System.Models;
using System.Security.Claims;
using System.Service.Interface;
using System.Services;
using Core.Bus;
using Core.Interfaces;
using Core.Notifications;
using DataAccess.Contexts;
using DataAccess.Models;
using Infrastructure.DomainService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace System.Service.Implements;
public class AccountService : DomainService, IAccountService
{
    private readonly IJwtFactory _jwtFactory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountService(
        ApplicationDbContext context,
        IJwtFactory jwtFactory,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _context = context;
        _jwtFactory = jwtFactory;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _bus = bus;
    }

    public async Task<UserRes?> ChangePasswordAsync(ChangePasswordReq req)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == req.UserId);
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("system.error.userNotFound", "system.message.pleaseCheckAgain"));
            return null;
        }
        var result = await _userManager.ChangePasswordAsync(user, req.OldPassword, req.NewPassword);
        if (result.Succeeded)
        {
            return new UserRes
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                DepartmentId = user.DepartmentId,
                PositionId = user.PositionId,
                DepartmentName = user.Department?.DepartmentName,
                PositionName = user.Position?.PositionName,
                ImageUrl = user.ImageUrl,
            };
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("system.error.changePasswordFailed", "system.message.pleaseCheckAgain"));
            return null;
        }
    }

    public async Task<LoginRes> LoginAsync(LoginReq req)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == req.UserName || x.UserName == req.UserName);
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("system.error.userNotFound", "system.message.pleaseCheckAgain"));
            return null;
        }
        var result = await _signInManager.PasswordSignInAsync(user, req.Password, false, false);
        if (result.Succeeded)
        {
            var token = await GenerateToken(user);
            var listExpiredToken = await _context.RefreshTokens
               .Where(d => d.UserId == user.Id)
               .Where(d => d.ExpiryDate < DateTime.UtcNow).ToListAsync();
            _context.RefreshTokens.RemoveRange(listExpiredToken);
            await _context.SaveChangesAsync();
            return new LoginRes
            {
                Token = token,
                User = new UserRes
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                }
            };

        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("system.error.loginFailed", "system.message.pleaseCheckAgain"));
            return null;
        }
    }

    public async Task<LoginRes> RefreshTokenAsync(RefreshTokenReq req)
    {
        var currentValidExistToken = await _context.RefreshTokens.FirstOrDefaultAsync(d => d.Token == req.RefreshToken && d.ExpiryDate > DateTime.UtcNow);
        if (currentValidExistToken is null)
        {
            return null;
        }
        var user = await _userManager.Users.Where(d => d.Id == currentValidExistToken.UserId)
            .FirstOrDefaultAsync();
        if (user is null || user.IsDelete || !user.IsActive)
        {
            return null;
        }
        var newToken = await RenewAccessToken(user, req.RefreshToken);
        return new LoginRes
        {
            Token = newToken,
            User = new UserRes
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user?.PhoneNumber,
                FullName = user?.FullName,
            }
        };

    }

    public async Task<LoginRes> RegisterAsync(RegisterReq req)
    {
        var user = new ApplicationUser
        {
            UserName = req.UserName,
            Email = req.Email,
            PhoneNumber = req?.PhoneNumber,
            FullName = req?.FullName,
            IsActive = true,
            IsDelete = false
        };
        var isExist = await _userManager.Users.AnyAsync(x => x.Email == req.Email || x.UserName == req.UserName);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("system.error.userExist", "system.message.pleaseCheckAgain"));
            return null;
        }
        var result = await _userManager.CreateAsync(user, req.Password);
        if (result.Succeeded)
        {
            var token = await GenerateToken(user);
            return new LoginRes
            {
                Token = token,
                User = new UserRes
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                }
            };
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("system.error.registerFailed", "system.message.pleaseCheckAgain"));
            return null;
        }

    }
    private async Task<TokenRes> GenerateToken(ApplicationUser appUser)
    {
        // Init ClaimsIdentity
        var claimsIdentity = new ClaimsIdentity();
        // claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, appUser.Email));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Id));

        // Get UserClaims
        var userClaims = await _userManager.GetClaimsAsync(appUser);
        claimsIdentity.AddClaims(userClaims);

        // Get UserRoles
        // var userRoles = await _userManager.GetRolesAsync(appUser);
        // claimsIdentity.AddClaims(userRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
        // ClaimsIdentity.DefaultRoleClaimType & ClaimTypes.Role is the same

        // Get RoleClaims
        // foreach (var userRole in userRoles)
        // {
        //     var role = await _roleManager.FindByNameAsync(userRole);
        //     var roleClaims = await _roleManager.GetClaimsAsync(role);
        //     claimsIdentity.AddClaims(roleClaims);
        // }

        // Generate access token
        var jwtToken = await _jwtFactory.GenerateJwtToken(claimsIdentity);

        // Add refresh token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = appUser.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            JwtId = jwtToken.JwtId
        };
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return new TokenRes
        {
            AccessToken = jwtToken.AccessToken,
            RefreshToken = refreshToken.Token,
        };
    }
    private async Task<TokenRes> RenewAccessToken(ApplicationUser appUser, string refreshToken)
    {
        // Init ClaimsIdentity
        var claimsIdentity = new ClaimsIdentity();
        // claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, appUser.Email));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Id));

        // Get UserClaims
        var userClaims = await _userManager.GetClaimsAsync(appUser);
        claimsIdentity.AddClaims(userClaims);

        // Get UserRoles
        var userRoles = await _userManager.GetRolesAsync(appUser);
        claimsIdentity.AddClaims(userRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
        // ClaimsIdentity.DefaultRoleClaimType & ClaimTypes.Role is the same

        // Get RoleClaims
        foreach (var userRole in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(userRole);
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            claimsIdentity.AddClaims(roleClaims);
        }

        // Generate access token
        var jwtToken = await _jwtFactory.GenerateJwtToken(claimsIdentity);

        return new TokenRes
        {
            AccessToken = jwtToken.AccessToken,
            RefreshToken = refreshToken,
        };
    }

}