using Microsoft.AspNetCore.Identity;
using SurveyBasket.Consts;

namespace SurveyBasket.Seeding;

public static class UserSeeder
{
    public static async Task SeedUserAsync(IServiceProvider serviceProvider)
    {

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();


        if (userManager.Users.Any())
            return;

        var adminEmail = DefaultUsers.AdminEmail;
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is not null)
            return;

        var password = configuration["Admin:Password"];
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Admin password not found in configuration (Admin:Password).");

        var newAdmin = new ApplicationUser
        {
            FirstName = DefaultUsers.FirstName,
            LastName = DefaultUsers.LastName,
            Email = adminEmail,
            UserName = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(newAdmin, password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create admin user: {errors}");
        }


        await userManager.AddToRoleAsync(newAdmin, DefaultRoles.Admin);
    }
}

