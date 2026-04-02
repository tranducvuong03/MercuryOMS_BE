using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class Product : AggregateRoot, IAuditableUser
    {
        private readonly List<ProductCategory> _categories = new();
        private readonly List<ProductImage> _images = new();
        private readonly List<ProductVariant> _variants = new();

        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal BasePrice { get; private set; }
        public bool IsActive { get; private set; }

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        public IReadOnlyCollection<ProductCategory> Categories => _categories.AsReadOnly();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        private Product() { }

        public Product(string name, decimal basePrice)
        {
            SetName(name);
            SetBasePrice(basePrice);

            Id = Guid.NewGuid();
            IsActive = true;
        }

        // Core
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên sản phẩm là bắt buộc.");

            Name = name.Trim();
        }

        public void SetDescription(string? description)
        {
            Description = description?.Trim();
        }

        public void SetBasePrice(decimal price)
        {
            if (price <= 0)
                throw new ArgumentException("Giá sản phẩm phải lớn hơn 0.");

            BasePrice = price;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        // Category
        public void AddCategory(Guid categoryId)
        {
            if (_categories.Any(x => x.CategoryId == categoryId))
                return;

            _categories.Add(new ProductCategory(Id, categoryId));
        }

        public void RemoveCategory(Guid categoryId)
        {
            var category = _categories.FirstOrDefault(x => x.CategoryId == categoryId);
            if (category != null)
                _categories.Remove(category);
        }

        // Image
        public void AddImage(string url, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Đường dẫn ảnh là bắt buộc.");

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException("URL ảnh không hợp lệ.");

            if (isPrimary)
            {
                foreach (var img in _images)
                    img.UnsetPrimary();
            }

            _images.Add(new ProductImage(Id, url, isPrimary));
        }

        public void RemoveImage(Guid imageId)
        {
            var img = _images.FirstOrDefault(x => x.Id == imageId);
            if (img != null)
                _images.Remove(img);
        }

        // Variant
        public void AddVariant(string sku, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU là bắt buộc.");

            if (price <= 0)
                throw new ArgumentException("Giá biến thể phải lớn hơn 0.");

            if (stock < 0)
                throw new ArgumentException("Số lượng tồn không hợp lệ.");

            if (_variants.Any(x => x.Sku == sku))
                throw new ArgumentException("SKU đã tồn tại.");

            _variants.Add(new ProductVariant(Id, sku, price, stock));
        }

        public void UpdateVariantStock(Guid variantId, int stock)
        {
            var variant = GetVariant(variantId);
            variant.SetStock(stock);
        }

        private ProductVariant GetVariant(Guid variantId)
        {
            var variant = _variants.FirstOrDefault(x => x.Id == variantId);
            if (variant == null)
                throw new ArgumentException("Không tìm thấy biến thể sản phẩm.");

            return variant;
        }
    }
}
