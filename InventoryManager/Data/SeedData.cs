using Microsoft.AspNetCore.Identity;
using InventoryManager.Constants;

namespace InventoryManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { Roles.Admin, Roles.Viewer };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Pull credentials from user secrets instead of hardcoding
            var adminEmail = configuration["AdminSettings:Email"];
            var adminPassword = configuration["AdminSettings:Password"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                throw new Exception("Admin credentials not found. Run: dotnet user-secrets set \"AdminSettings:Email\" and \"AdminSettings:Password\"");
            }
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true  // Skip email confirmation for the seed user
                };

                // Create the user with a password
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
            }
        }
    }
}