using EduBridge.Abstractions.Consts;
using EduBridge.Entities;
using Microsoft.AspNetCore.Identity;

namespace EduBridge.Persistence.Seed;
public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var roles = new[]
        {
            DefaultRoles.Admin,
            DefaultRoles.Student,
            DefaultRoles.TA
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
            }
        }
    }
}