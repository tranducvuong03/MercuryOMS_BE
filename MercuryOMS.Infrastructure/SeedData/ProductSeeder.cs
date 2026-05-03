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

            var categories = await context.Categories
                .ToDictionaryAsync(c => c.Name, c => c.Id);

            Guid GetCategory(string name) => categories[name];

            var products = new List<Product>();

            void AddProduct(
                string name,
                string description,
                string categoryName,
                string imageUrl,
                params (string sku, decimal originalPrice, decimal? discountPrice, string color, string? size, int stock)[] variants)
            {
                var product = new Product(name);

                product.SetDescription(description);
                product.AddCategory(GetCategory(categoryName));
                product.AddImage(imageUrl, true);

                foreach (var v in variants)
                {
                    var sku = $"{v.sku}-{Guid.NewGuid().ToString()[..6]}";

                    product.AddVariant(
                        sku,
                        v.originalPrice,
                        v.discountPrice,
                        v.color,
                        v.stock,
                        null,
                        v.size
                    );

                    var variant = product.Variants.First(x => x.Sku == sku);

                    context.Inventories.Add(new Inventory(variant.Id, v.stock));
                }

                products.Add(product);
            }

            AddProduct(
                "iPhone 15",
                "Điện thoại cao cấp Apple",
                "Điện tử",
                "https://picsum.photos/seed/p1/600/600",
                ("IP15-128", 20000000, 19500000, "Black", "128GB", 10),
                ("IP15-256", 23000000, null, "Silver", "256GB", 5)
            );

            AddProduct(
                "Samsung Galaxy S23",
                "Android flagship",
                "Điện tử",
                "https://picsum.photos/seed/p2/600/600",
                ("S23-128", 18000000, 17500000, "Black", "128GB", 10),
                ("S23-256", 21000000, null, "Green", "256GB", 5)
            );

            AddProduct(
                "MacBook Air M2",
                "Laptop mỏng nhẹ",
                "Điện tử",
                "https://picsum.photos/seed/p3/600/600",
                ("MBA-8", 28000000, 27000000, "Gray", "8GB", 8),
                ("MBA-16", 32000000, null, "Gray", "16GB", 3)
            );

            AddProduct(
                "Dell XPS 13",
                "Laptop cao cấp",
                "Điện tử",
                "https://picsum.photos/seed/p4/600/600",
                ("XPS-8", 30000000, 29000000, "Silver", "8GB", 6),
                ("XPS-16", 34000000, null, "Silver", "16GB", 4)
            );

            AddProduct(
                "AirPods Pro",
                "Tai nghe chống ồn",
                "Điện tử",
                "https://picsum.photos/seed/p5/600/600",
                ("APP", 6000000, 5500000, "White", null, 20)
            );

            AddProduct(
                "Logitech MX Master 3",
                "Chuột cao cấp",
                "Điện tử",
                "https://picsum.photos/seed/p6/600/600",
                ("MXM3", 2500000, 2300000, "Black", null, 15)
            );

            AddProduct(
                "Keychron K6",
                "Bàn phím cơ",
                "Điện tử",
                "https://picsum.photos/seed/p7/600/600",
                ("K6", 2200000, null, "Black", null, 10)
            );

            AddProduct(
                "LG 27 inch",
                "Màn hình IPS",
                "Điện tử",
                "https://picsum.photos/seed/p8/600/600",
                ("LG27", 5000000, 4800000, "Black", "27inch", 7)
            );

            AddProduct(
                "SSD Samsung 1TB",
                "Ổ cứng SSD",
                "Điện tử",
                "https://picsum.photos/seed/p9/600/600",
                ("SSD1TB", 2000000, null, "Black", "1TB", 25)
            );

            AddProduct(
                "JBL Flip 6",
                "Loa bluetooth",
                "Điện tử",
                "https://picsum.photos/seed/p10/600/600",
                ("JBL6", 3000000, 2700000, "Blue", null, 12)
            );

            AddProduct(
                "Áo thun basic",
                "Cotton 100%",
                "Thời trang",
                "https://picsum.photos/seed/p11/600/600",
                ("TS-BLACK-M", 150000, 120000, "Black", "M", 50),
                ("TS-WHITE-L", 150000, null, "White", "L", 40)
            );

            AddProduct(
                "Quần jeans",
                "Slim fit",
                "Thời trang",
                "https://picsum.photos/seed/p12/600/600",
                ("JEANS-32", 400000, 350000, "Blue", "32", 30)
            );

            AddProduct(
                "Áo hoodie",
                "Áo nỉ",
                "Thời trang",
                "https://picsum.photos/seed/p13/600/600",
                ("HOODIE-M", 350000, null, "Black", "M", 20)
            );

            AddProduct(
                "Giày sneaker",
                "Giày thể thao",
                "Thời trang",
                "https://picsum.photos/seed/p14/600/600",
                ("SNK-42", 900000, 850000, "White", "42", 15),
                ("SNK-43", 900000, null, "Black", "43", 15)
            );

            AddProduct(
                "Áo sơ mi",
                "Công sở",
                "Thời trang",
                "https://picsum.photos/seed/p15/600/600",
                ("SHIRT-M", 250000, 220000, "White", "M", 35)
            );

            AddProduct(
                "Váy nữ",
                "Thời trang nữ",
                "Thời trang",
                "https://picsum.photos/seed/p16/600/600",
                ("DRESS-S", 500000, 450000, "Red", "S", 18)
            );

            AddProduct(
                "Áo khoác denim",
                "Jean jacket",
                "Thời trang",
                "https://picsum.photos/seed/p17/600/600",
                ("DENIM-M", 600000, null, "Blue", "M", 12)
            );

            AddProduct(
                "Balo",
                "Tiện dụng",
                "Thời trang",
                "https://picsum.photos/seed/p18/600/600",
                ("BALO", 300000, 250000, "Black", null, 25)
            );

            AddProduct(
                "Nón lưỡi trai",
                "Casual",
                "Thời trang",
                "https://picsum.photos/seed/p19/600/600",
                ("CAP", 120000, null, "Black", null, 40)
            );

            AddProduct(
                "Thắt lưng da",
                "Da thật",
                "Thời trang",
                "https://picsum.photos/seed/p20/600/600",
                ("BELT", 200000, 180000, "Brown", null, 22)
            );

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}