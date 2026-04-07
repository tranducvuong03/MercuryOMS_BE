using MercuryOMS.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MercuryOMS.Infrastructure.SeedData
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDataAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            // Role
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await RoleSeeder.SeedAsync(roleManager);

            // Category
            var context = services.GetRequiredService<ApplicationDbContext>();
            await CategorySeeder.SeedAsync(context);
            await ProductSeeder.SeedAsync(context);
        }
    }
}
