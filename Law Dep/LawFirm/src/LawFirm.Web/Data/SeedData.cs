using LawFirm.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Data;

public static class SeedData
{
    public const string AdminRole = "Admin";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();
        var config = sp.GetRequiredService<IConfiguration>();

        await db.Database.MigrateAsync();

        await SeedAdminAsync(sp, config);
        await SeedContentAsync(db);
    }

    private static async Task SeedAdminAsync(IServiceProvider sp, IConfiguration config)
    {
        var roles = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var users = sp.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roles.RoleExistsAsync(AdminRole))
            await roles.CreateAsync(new IdentityRole(AdminRole));

        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        if (await users.FindByEmailAsync(email) is null)
        {
            var admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await users.CreateAsync(admin, password);
            if (result.Succeeded) await users.AddToRoleAsync(admin, AdminRole);
        }
    }

    private static async Task SeedContentAsync(AppDbContext db)
    {
        if (await db.PracticeAreas.AnyAsync()) return;

        var corporate = new PracticeArea { Name = "Corporate & Commercial", Description = "Company formation, shareholder agreements, mergers and day-to-day commercial contracts." };
        var employment = new PracticeArea { Name = "Employment", Description = "Employment contracts, dismissals, settlement agreements and workplace investigations." };
        var family = new PracticeArea { Name = "Family Law", Description = "Divorce, child arrangements, prenuptial agreements and financial settlements." };
        var property = new PracticeArea { Name = "Real Estate", Description = "Purchases, leases, development agreements and landlord–tenant disputes." };
        var ip = new PracticeArea { Name = "Intellectual Property", Description = "Trademarks, licensing, copyright and protection of trade secrets." };
        var disputes = new PracticeArea { Name = "Dispute Resolution", Description = "Litigation, mediation and arbitration for businesses and individuals." };

        db.PracticeAreas.AddRange(corporate, employment, family, property, ip, disputes);

        db.Lawyers.AddRange(
            new Lawyer
            {
                FullName = "Eleanor Calder", Title = "Managing Partner", Email = "eleanor@calderrowe.test",
                Bio = "Eleanor advises founders and boards on corporate structure, financing and cross-border transactions. She has led the firm since 2014.",
                PracticeAreas = { corporate, ip }
            },
            new Lawyer
            {
                FullName = "Marcus Rowe", Title = "Partner", Email = "marcus@calderrowe.test",
                Bio = "Marcus handles commercial disputes from first letter to final hearing, with a focus on contract and construction claims.",
                PracticeAreas = { disputes, property }
            },
            new Lawyer
            {
                FullName = "Priya Nair", Title = "Senior Associate", Email = "priya@calderrowe.test",
                Bio = "Priya represents both employers and employees in dismissal, discrimination and restructuring matters.",
                PracticeAreas = { employment, disputes }
            },
            new Lawyer
            {
                FullName = "Daniel Okafor", Title = "Associate", Email = "daniel@calderrowe.test",
                Bio = "Daniel works with families on divorce and child arrangements, and helps first-time buyers through residential purchases.",
                PracticeAreas = { family, property }
            });

        await db.SaveChangesAsync();
    }
}
