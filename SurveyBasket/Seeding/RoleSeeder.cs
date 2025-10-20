using Microsoft.AspNetCore.Identity;
using SurveyBasket.Consts;
namespace Seeding;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        IReadOnlyList<string> roleNames = DefaultRoles.GetAll();

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    IsDefault = (roleName == "Member"),
                    Name = roleName,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
        }
    }
}