using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Infrastructure.Identity;
using PharmacySystem.Infrastructure.Persistence;

namespace PharmacySystem.Infrastructure.Seeding;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope       = services.CreateScope();
        var context           = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager       = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager       = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger            = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // ── Run pending migrations ────────────────────────────────────
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration applied.");

            // ── Seed roles ────────────────────────────────────────────────
            foreach (var role in Enum.GetNames<UserRole>())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Role '{Role}' seeded.", role);
                }
            }

            // ── Seed default Admin user ───────────────────────────────────
            const string adminEmail    = "admin@pharmacy.com";
            const string adminPassword = "Admin@1234";

            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser
                {
                    FullName       = "System Administrator",
                    UserName       = adminEmail,
                    Email          = adminEmail,
                    EmailConfirmed = true,
                    IsActive       = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, nameof(UserRole.Admin));
                    logger.LogInformation("Default admin user seeded → {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Failed to seed admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // ── Seed expense categories ───────────────────────────────────
            if (!await context.ExpenseCategories.AnyAsync())
            {
                context.ExpenseCategories.AddRange(
                    new ExpenseCategory { Name = "Rent",        Description = "Office / store rent" },
                    new ExpenseCategory { Name = "Utilities",   Description = "Electricity, water, internet" },
                    new ExpenseCategory { Name = "Salaries",    Description = "Staff salaries and wages" },
                    new ExpenseCategory { Name = "Maintenance", Description = "Equipment maintenance" },
                    new ExpenseCategory { Name = "Other",       Description = "Miscellaneous expenses" }
                );
                await context.SaveChangesAsync();
                logger.LogInformation("Expense categories seeded.");
            }

            // ── Seed medicine categories ──────────────────────────────────
            if (!await context.Categories.AnyAsync())
            {
                context.Categories.AddRange(
                    new Category { Name = "Antibiotics",      Description = "Antibiotic medications" },
                    new Category { Name = "Analgesics",       Description = "Pain relievers" },
                    new Category { Name = "Vitamins",         Description = "Vitamins and supplements" },
                    new Category { Name = "Antidiabetics",    Description = "Diabetes medications" },
                    new Category { Name = "Antihypertensives",Description = "Blood pressure medications" },
                    new Category { Name = "Antihistamines",   Description = "Allergy medications" },
                    new Category { Name = "Antacids",         Description = "Stomach and digestive medications" },
                    new Category { Name = "Topical",          Description = "Creams, ointments, and gels" }
                );
                await context.SaveChangesAsync();
                logger.LogInformation("Medicine categories seeded.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding.");
            throw;
        }
    }
}
