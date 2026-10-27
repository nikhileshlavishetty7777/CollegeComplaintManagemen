using CollegeComplaintManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeComplaintManagement.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<ApplicationDbContext>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var config = provider.GetRequiredService<IConfiguration>();
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

            // The schema is created from EF Core migrations. Fail early with a clear message
            // if the Migrations folder has not been generated yet.
            if (!context.Database.GetMigrations().Any())
            {
                throw new InvalidOperationException(
                    "No EF Core migrations were found. Run:  dotnet ef migrations add InitialCreate  " +
                    "(or run setup.ps1) and then start the application again.");
            }

            // Creates the database if it does not exist and applies pending migrations
            await context.Database.MigrateAsync();

            // Create roles
            foreach (var role in new[] { AppRoles.Admin, AppRoles.Student })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Create the first admin from configuration (user-secrets / environment variables)
            var adminEmail = config["AdminSeed:Email"];
            var adminPassword = config["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("AdminSeed:Email / AdminSeed:Password not configured. No admin account created.");
                return;
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    Department = "Administration",
                    RegisteredAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, AppRoles.Admin);
                    logger.LogInformation("Initial admin account created.");
                }
                else
                {
                    logger.LogError("Admin creation failed: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
