using MercuryOMS.Domain.Entities;
using MercuryOMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Infrastructure.SeedData
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

            Guid GetCategory(string name) => categories[name];

            var products = new List<Product>();

            void AddProduct(
                string name,
                decimal price,
                string description,
                string categoryName,
                string imageUrl,
                params (string sku, decimal price, int stock)[] variants)
            {
                var product = new Product(name, price);

                product.SetDescription(description);
                product.AddCategory(GetCategory(categoryName));
                product.AddImage(imageUrl, true);

                foreach (var v in variants)
                {
                    var sku = $"{v.sku}-{Guid.NewGuid().ToString()[..6]}";
                    product.AddVariant(sku, v.price, v.stock);
                }

                products.Add(product);
            }

            AddProduct("iPhone 15", 20000000, "Điện thoại cao cấp Apple", "Điện tử",
                "https://example.com/iphone15.jpg",
                ("IP15-128", 20000000, 10),
                ("IP15-256", 23000000, 5));

            AddProduct("Samsung Galaxy S23", 18000000, "Android flagship", "Điện tử",
                "https://example.com/s23.jpg",
                ("S23-128", 18000000, 10),
                ("S23-256", 21000000, 5));

            AddProduct("MacBook Air M2", 28000000, "Laptop mỏng nhẹ", "Điện tử",
                "https://example.com/macbook.jpg",
                ("MBA-8", 28000000, 8),
                ("MBA-16", 32000000, 3));

            AddProduct("Dell XPS 13", 30000000, "Laptop cao cấp", "Điện tử",
                "https://example.com/xps13.jpg",
                ("XPS-8", 30000000, 6),
                ("XPS-16", 34000000, 4));

            AddProduct("AirPods Pro", 6000000, "Tai nghe chống ồn", "Điện tử",
                "https://example.com/airpods.jpg",
                ("APP", 6000000, 20));

            AddProduct("Logitech MX Master 3", 2500000, "Chuột cao cấp", "Điện tử",
                "https://example.com/mouse.jpg",
                ("MXM3", 2500000, 15));

            AddProduct("Keychron K6", 2200000, "Bàn phím cơ", "Điện tử",
                "https://example.com/keychron.jpg",
                ("K6", 2200000, 10));

            AddProduct("LG 27 inch", 5000000, "Màn hình IPS", "Điện tử",
                "https://example.com/lg.jpg",
                ("LG27", 5000000, 7));

            AddProduct("SSD Samsung 1TB", 2000000, "Ổ cứng SSD", "Điện tử",
                "https://example.com/ssd.jpg",
                ("SSD1TB", 2000000, 25));

            AddProduct("JBL Flip 6", 3000000, "Loa bluetooth", "Điện tử",
                "https://example.com/jbl.jpg",
                ("JBL6", 3000000, 12));

            AddProduct("Áo thun basic", 150000, "Cotton 100%", "Thời trang",
                "https://example.com/tshirt.jpg",
                ("TS-BLACK-M", 150000, 50),
                ("TS-WHITE-L", 150000, 40));

            AddProduct("Quần jeans", 400000, "Slim fit", "Thời trang",
                "https://example.com/jeans.jpg",
                ("JEANS", 400000, 30));

            AddProduct("Áo hoodie", 350000, "Áo nỉ", "Thời trang",
                "https://example.com/hoodie.jpg",
                ("HOODIE", 350000, 20));

            AddProduct("Giày sneaker", 900000, "Giày thể thao", "Thời trang",
                "https://example.com/sneaker.jpg",
                ("SNK", 900000, 15));

            AddProduct("Áo sơ mi", 250000, "Công sở", "Thời trang",
                "https://example.com/shirt.jpg",
                ("SHIRT", 250000, 35));

            AddProduct("Váy nữ", 500000, "Thời trang nữ", "Thời trang",
                "https://example.com/dress.jpg",
                ("DRESS", 500000, 18));

            AddProduct("Áo khoác denim", 600000, "Jean jacket", "Thời trang",
                "https://example.com/denim.jpg",
                ("DENIM", 600000, 12));

            AddProduct("Balo", 300000, "Tiện dụng", "Thời trang",
                "https://example.com/backpack.jpg",
                ("BALO", 300000, 25));

            AddProduct("Nón lưỡi trai", 120000, "Casual", "Thời trang",
                "https://example.com/cap.jpg",
                ("CAP", 120000, 40));

            AddProduct("Thắt lưng da", 200000, "Da thật", "Thời trang",
                "https://example.com/belt.jpg",
                ("BELT", 200000, 22));

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}