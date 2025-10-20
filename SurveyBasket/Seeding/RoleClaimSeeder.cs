using Microsoft.AspNetCore.Identity;
using SurveyBasket.Consts;
using System.Security.Claims;

namespace SurveyBasket.Seeding;

public static class RoleClaimSeeder
{
    public static async Task SeedRoleClaimsAsync(IServiceProvider serviceProvider)
    {


        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var role = await roleManager.FindByNameAsync(DefaultRoles.Admin);

        if (role is null)
            throw new Exception("Admin role not found. Make sure RoleSeeder ran before RoleClaimSeeder.");

        var allPermissions = Permissions.GetAllPermissions();

        var existingClaims = await roleManager.GetClaimsAsync(role);
        var existingValues = existingClaims.Select(c => c.Value).ToHashSet();

        foreach (var permission in allPermissions)
        {
            if (!existingValues.Contains(permission))
            {
                await roleManager.AddClaimAsync(role, new Claim(Permissions.Type, permission));
            }
        }
    }
}

