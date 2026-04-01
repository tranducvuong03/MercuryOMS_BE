using MercuryOMS.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace MercuryOMS.Infrastructure.SeedData
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[]
            {
                Role.Admin,
                Role.Member,
                Role.Seller
            };

            foreach (var role in roles)
            {
                var exists = await roleManager.RoleExistsAsync(role);
                if (!exists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}