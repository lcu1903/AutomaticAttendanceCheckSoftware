using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class ApplicationDataInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<ApplicationDbContext>();

        string[] roles = new string[] {"Admin", "Manager","Student",  "Teacher",};

        foreach (string role in roles)
        {
            var roleStore = new RoleStore<IdentityRole>(context);

            if (!context.Roles.Any(r => r.Name == role))
            {
                roleStore.CreateAsync(new IdentityRole(role));
            }
        }


        var user = new ApplicationUser
        {
            Id = "ADMIN",
            FirstName = "XXXX",
            LastName = "XXXX",
            Email = "xxxx@example.com",
            NormalizedEmail = "XXXX@EXAMPLE.COM",
            UserName = "Owner",
            NormalizedUserName = "OWNER",
            PhoneNumber = "+111111111111",
            SecurityStamp = Guid.NewGuid().ToString("D")
        };


        if (!context.Users.Any(u => u.UserName == user.UserName))
        {
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user,"secret");
            user.PasswordHash = hashed;

            var userStore = new UserStore<ApplicationUser>(context);
            var result = userStore.CreateAsync(user);

        }

        AssignRoles(serviceProvider, user.Email, roles);

        context.SaveChangesAsync();
    }

    public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
    {
        UserManager<ApplicationUser> _userManager = services.GetService<UserManager<ApplicationUser>>();
        ApplicationUser user = await _userManager.FindByEmailAsync(email);
        var result = await _userManager.AddToRolesAsync(user, roles);

        return result;
    }

}