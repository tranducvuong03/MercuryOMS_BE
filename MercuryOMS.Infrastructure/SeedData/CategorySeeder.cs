using MercuryOMS.Domain.Entities;
using MercuryOMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Infrastructure.SeedData
{
    public static class CategorySeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var categories = new[]
            {
                new { Name = "Điện tử", Description = "Thiết bị điện tử và phụ kiện" },
                new { Name = "Thời trang", Description = "Quần áo và phụ kiện thời trang" },
                new { Name = "Thực phẩm & Đồ uống", Description = "Các sản phẩm thực phẩm và đồ uống" },
                new { Name = "Nhà cửa & Nội thất", Description = "Đồ nội thất và trang trí nhà cửa" },
                new { Name = "Thể thao", Description = "Dụng cụ và phụ kiện thể thao" },
                new { Name = "Sách & Văn phòng phẩm", Description = "Sách, bút và dụng cụ văn phòng" },
                new { Name = "Mỹ phẩm & Làm đẹp", Description = "Sản phẩm chăm sóc sắc đẹp" },
            };

            foreach (var item in categories)
            {
                var exists = await context.Categories.AnyAsync(c => c.Name == item.Name);
                if (!exists)
                {
                    var category = new Category(item.Name, item.Description);
                    await context.Categories.AddAsync(category);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}