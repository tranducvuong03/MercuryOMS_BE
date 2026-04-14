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
                params (string sku, decimal price, string color, string? size, int stock)[] variants)                           
            {
                var product = new Product(name, price);

                product.SetDescription(description);
                product.AddCategory(GetCategory(categoryName));
                product.AddImage(imageUrl, true);

                foreach (var v in variants)
                {
                    var sku = $"{v.sku}-{Guid.NewGuid().ToString()[..6]}";

                    product.AddVariant(sku, v.price, v.color, v.stock, v.size);

                    var variant = product.Variants.First(x => x.Sku == sku);

                    context.Inventories.Add(new Inventory(variant.Id, v.stock));
                }

                products.Add(product);
            }

            AddProduct("iPhone 15", 20000000, "Điện thoại cao cấp Apple", "Điện tử",
                "https://example.com/iphone15.jpg",
                ("IP15-128", 20000000, "Black", "128GB", 10),
                ("IP15-256", 23000000, "Silver", "256GB", 5));

            AddProduct("Samsung Galaxy S23", 18000000, "Android flagship", "Điện tử",
                "https://example.com/s23.jpg",
                ("S23-128", 18000000, "Black", "128GB", 10),
                ("S23-256", 21000000, "Green", "256GB", 5));

            AddProduct("MacBook Air M2", 28000000, "Laptop mỏng nhẹ", "Điện tử",
                "https://example.com/macbook.jpg",
                ("MBA-8", 28000000, "Gray", "8GB", 8),
                ("MBA-16", 32000000, "Gray", "16GB", 3));

            AddProduct("Dell XPS 13", 30000000, "Laptop cao cấp", "Điện tử",
                "https://example.com/xps13.jpg",
                ("XPS-8", 30000000, "Silver", "8GB", 6),
                ("XPS-16", 34000000, "Silver", "16GB", 4));

            AddProduct("AirPods Pro", 6000000, "Tai nghe chống ồn", "Điện tử",
                "https://example.com/airpods.jpg",
                ("APP", 6000000, "White", null, 20));

            AddProduct("Logitech MX Master 3", 2500000, "Chuột cao cấp", "Điện tử",
                "https://example.com/mouse.jpg",
                ("MXM3", 2500000, "Black", null, 15));

            AddProduct("Keychron K6", 2200000, "Bàn phím cơ", "Điện tử",
                "https://example.com/keychron.jpg",
                ("K6", 2200000, "Black", null, 10));

            AddProduct("LG 27 inch", 5000000, "Màn hình IPS", "Điện tử",
                "https://example.com/lg.jpg",
                ("LG27", 5000000, "Black", "27inch", 7));

            AddProduct("SSD Samsung 1TB", 2000000, "Ổ cứng SSD", "Điện tử",
                "https://example.com/ssd.jpg",
                ("SSD1TB", 2000000, "Black", "1TB", 25));

            AddProduct("JBL Flip 6", 3000000, "Loa bluetooth", "Điện tử",
                "https://example.com/jbl.jpg",
                ("JBL6", 3000000, "Blue", null, 12));

            AddProduct("Áo thun basic", 150000, "Cotton 100%", "Thời trang",
                "https://example.com/tshirt.jpg",
                ("TS-BLACK-M", 150000, "Black", "M", 50),
                ("TS-WHITE-L", 150000, "White", "L", 40));

            AddProduct("Quần jeans", 400000, "Slim fit", "Thời trang",
                "https://example.com/jeans.jpg",
                ("JEANS-32", 400000, "Blue", "32", 30));

            AddProduct("Áo hoodie", 350000, "Áo nỉ", "Thời trang",
                "https://example.com/hoodie.jpg",
                ("HOODIE-M", 350000, "Black", "M", 20));

            AddProduct("Giày sneaker", 900000, "Giày thể thao", "Thời trang",
                "https://example.com/sneaker.jpg",
                ("SNK-42", 900000, "White", "42", 15),
                ("SNK-43", 900000, "Black", "43", 15));

            AddProduct("Áo sơ mi", 250000, "Công sở", "Thời trang",
                "https://example.com/shirt.jpg",
                ("SHIRT-M", 250000, "White", "M", 35));

            AddProduct("Váy nữ", 500000, "Thời trang nữ", "Thời trang",
                "https://example.com/dress.jpg",
                ("DRESS-S", 500000, "Red", "S", 18));

            AddProduct("Áo khoác denim", 600000, "Jean jacket", "Thời trang",
                "https://example.com/denim.jpg",
                ("DENIM-M", 600000, "Blue", "M", 12));

            AddProduct("Balo", 300000, "Tiện dụng", "Thời trang",
                "https://example.com/backpack.jpg",
                ("BALO", 300000, "Black", null, 25));

            AddProduct("Nón lưỡi trai", 120000, "Casual", "Thời trang",
                "https://example.com/cap.jpg",
                ("CAP", 120000, "Black", null, 40));

            AddProduct("Thắt lưng da", 200000, "Da thật", "Thời trang",
                "https://example.com/belt.jpg",
                ("BELT", 200000, "Brown", null,  22));

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}