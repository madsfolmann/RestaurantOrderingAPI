using Microsoft.AspNetCore.Identity;
using RestaurantOrderingAPI.Application.Common.Constants;
using RestaurantOrderingAPI.Features.Users;

namespace RestaurantOrderingAPI.Application.Data;

public class AppDbContextDataSeed
{
    public static async Task SeedTestUsersAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Seed test users
        var users = new List<(List<string> Roles, string Email)>
        {
            ([Role.Customer], "customer@example.com"),
            ([Role.Chef], "chef@example.com"),
            ([Role.Staff], "staff@example.com"),
            ([Role.Admin], "admin@example.com"),
            ([Role.SuperUser, Role.Admin], "superuser@example.com"),
        };
        foreach (var (roles, email) in users)
        {
            var user = new User
            {
                UserName = email,
                Email = email,
            };

            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                await userManager.CreateAsync(user, "Test1234.");
                foreach (var role in roles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}