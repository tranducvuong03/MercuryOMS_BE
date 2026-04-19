using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Entities.Notification;
using MercuryOMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<Seller> Sellers => Set<Seller>();
        public DbSet<SellerBalance> SellerBalances => Set<SellerBalance>();
        public DbSet<SellerProduct> SellerProducts => Set<SellerProduct>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
        public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();
        public DbSet<Notification> Notifications => Set<Notification>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureCart(builder);
            ConfigureProduct(builder);
            ConfigureCategory(builder);
            ConfigureInventory(builder);
            ConfigureOrder(builder);
            ConfigureSeller(builder);
            ConfigureShipment(builder);
            ConfigureSupportTicket(builder);
        }

        private static void ConfigureCart(ModelBuilder builder)
        {
            builder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.UserId).IsRequired();

                entity.HasMany(c => c.Items)
                      .WithOne()
                      .HasForeignKey(ci => ci.Id) 
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);

                entity.Property(ci => ci.Id).IsRequired();
                entity.Property(ci => ci.ProductId).IsRequired();
                entity.Property(ci => ci.Quantity).IsRequired();

                entity.HasIndex(ci => ci.Id);
                entity.HasIndex(ci => ci.ProductId);
            });
        }

        private static void ConfigureProduct(ModelBuilder builder)
        {
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.BasePrice).IsRequired();

                entity.HasMany(p => p.Images)
                      .WithOne()
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Variants)
                      .WithOne()
                      .HasForeignKey(pv => pv.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

                entity.HasIndex(pc => pc.CategoryId);
            });
        }

        private static void ConfigureCategory(ModelBuilder builder)
        {
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name).IsRequired();
            });
        }

        private static void ConfigureInventory(ModelBuilder builder)
        {
            builder.Entity<Inventory>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.VariantId)
                      .IsRequired();

                entity.HasMany<InventoryLog>()
                      .WithOne()
                      .HasForeignKey(il => il.InventoryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(i => i.VariantId)
                      .IsUnique();
            });

            builder.Entity<InventoryLog>(entity =>
            {
                entity.HasKey(il => il.Id);

                entity.Property(il => il.Quantity).IsRequired();
                entity.Property(il => il.Type).IsRequired();
            });
        }

        private static void ConfigureOrder(ModelBuilder builder)
        {
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.UserId).IsRequired();

                entity.HasMany(o => o.Items)
                      .WithOne()
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.OrderId).IsRequired();
                entity.Property(oi => oi.ProductVariantId).IsRequired();
                entity.Property(oi => oi.Quantity).IsRequired();
                entity.Property(oi => oi.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.HasIndex(oi => oi.OrderId);
                entity.HasIndex(oi => oi.ProductVariantId);

                entity.HasIndex(oi => new { oi.OrderId, oi.ProductVariantId })
                    .IsUnique();
            });

            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.OrderId).IsRequired();

                entity.HasOne<Order>()
                      .WithOne()
                      .HasForeignKey<Payment>(p => p.OrderId);
            });

            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Amount).IsRequired();
                entity.Property(t => t.Type).IsRequired();
                entity.Property(t => t.PaymentId).IsRequired();

                entity.HasIndex(t => t.PaymentId);
            });
        }

        private static void ConfigureSeller(ModelBuilder builder)
        {
            builder.Entity<Seller>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasOne(s => s.Balance)
                      .WithOne()
                      .HasForeignKey<SellerBalance>(sb => sb.SellerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.Products)
                      .WithOne()
                      .HasForeignKey(sp => sp.SellerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicationUser>()
                      .WithOne()
                      .HasForeignKey<Seller>(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SellerProduct>(entity =>
            {
                entity.HasKey(sp => new { sp.SellerId, sp.ProductId });

                entity.Property(sp => sp.CommissionRate)
                      .IsRequired()
                      .HasPrecision(5, 2);

                entity.HasIndex(sp => sp.ProductId);
            });

            builder.Entity<SellerBalance>(entity =>
            {
                entity.HasKey(sb => sb.SellerId);

                entity.Property(sb => sb.SellerId).IsRequired();
                entity.Property(sb => sb.Amount).IsRequired();

                entity.HasIndex(sb => sb.SellerId).IsUnique();
            });
        }

        private static void ConfigureShipment(ModelBuilder builder)
        {
            builder.Entity<Shipment>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.OrderId).IsRequired();
                entity.Property(s => s.Status).IsRequired();

                entity.HasIndex(s => s.OrderId);

                entity.HasOne<Order>()
                      .WithOne()
                      .HasForeignKey<Shipment>(s => s.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureSupportTicket(ModelBuilder builder)
        {
            builder.Entity<SupportTicket>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Description)
                      .IsRequired();

                entity.Property(t => t.CreatedByUserId)
                      .IsRequired();

                entity.Property(t => t.AssignedStaffId);

                entity.Property(t => t.Status)
                      .IsRequired()
                      .HasConversion<int>();

                entity.Property(t => t.CreatedAt)
                      .IsRequired();

                entity.Property(t => t.ClosedAt);

                entity.HasMany(t => t.Messages)
                      .WithOne()
                      .HasForeignKey(m => m.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<TicketMessage>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.TicketId)
                      .IsRequired();

                entity.Property(m => m.SenderId)
                      .IsRequired();

                entity.Property(m => m.Content)
                      .IsRequired();

                entity.Property(m => m.SentAt)
                      .IsRequired();

                entity.HasIndex(m => m.TicketId);
            });
        }
    }
}
