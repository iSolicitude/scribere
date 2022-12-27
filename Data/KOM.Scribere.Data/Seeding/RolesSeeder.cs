namespace KOM.Scribere.Data.Seeding;

using System;
using System.Linq;
using System.Threading.Tasks;

using KOM.Scribere.Common;
using KOM.Scribere.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

internal class RolesSeeder : ISeeder
{
    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        await SeedRoleAsync(roleManager, userManager, GlobalConstants.AdministratorRoleName);
        await SeedRoleAsync(roleManager, userManager, GlobalConstants.UserRoleName);
    }

    private static async Task SeedRoleAsync(RoleManager<Role> roleManager, UserManager<User> userManager, string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        if (role == null)
        {
            var result = await roleManager.CreateAsync(new Role(roleName));

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.Users.AnyAsync())
        {
            var user = new User
            {
                UserName = "kingyadid",
                FirstName = "Domornie",
                LastName = "Nelson",
                Email = "yadid@isolicitude.net",
                CreatedOn = DateTime.Now,
                EmailConfirmed = true,
            };

            var password = "R3K0n1335e";

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
            }
        }
    }
}
