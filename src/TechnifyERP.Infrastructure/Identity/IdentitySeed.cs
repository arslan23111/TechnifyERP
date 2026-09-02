using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Infrastructure.Identity;

public static class IdentitySeed
{
    private static readonly string[] Roles = ["SuperAdmin", "Faculty", "Student"];

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in Roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                EnsureSucceeded(result, $"create role '{roleName}'");
            }
        }

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Super Admin",
                UserType = UserType.SuperAdmin,
                AccountStatus = AccountStatus.Active
            };
            EnsureSucceeded(await userManager.CreateAsync(admin, password), "create the development admin");
        }

        admin.FullName = "Super Admin";
        admin.UserType = UserType.SuperAdmin;
        admin.AccountStatus = AccountStatus.Active;
        admin.EmailConfirmed = true;
        EnsureSucceeded(await userManager.UpdateAsync(admin), "update the development admin");

        if (!await userManager.IsInRoleAsync(admin, "SuperAdmin"))
        {
            EnsureSucceeded(await userManager.AddToRoleAsync(admin, "SuperAdmin"), "assign the SuperAdmin role");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string action)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Could not {action}: {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }
    }
}
