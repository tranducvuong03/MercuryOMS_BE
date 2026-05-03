using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Events;
using MercuryOMS.Domain.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace MercuryOMS.Domain.Entities
{
    public class Product : AggregateRoot, IAuditableUser
    {
        private readonly List<ProductCategory> _categories = new();
        private readonly List<ProductImage> _images = new();
        private readonly List<ProductVariant> _variants = new();

        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string? Description { get; private set; }

        public bool IsActive { get; private set; }
        public string? Brand { get; private set; }
        public string? Badge { get; }

        public string? Thumbnail => _images.FirstOrDefault(x => x.IsPrimary)?.Url;

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        public IReadOnlyCollection<ProductCategory> Categories => _categories.AsReadOnly();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        public decimal MinPrice =>
            _variants.Any()
                ? _variants.Min(x => x.DiscountPrice ?? x.OriginalPrice)
                : 0;

        public decimal MaxPrice =>
            _variants.Any()
                ? _variants.Max(x => x.DiscountPrice ?? x.OriginalPrice)
                : 0;

        private Product() { }

        public Product(string name)
        {
            Id = Guid.NewGuid();

            SetName(name);
            SetSlug(name);

            IsActive = true;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên sản phẩm là bắt buộc.");

            Name = name.Trim();
        }

        public void SetSlug(string name)
        {
            Slug = GenerateSlug(name);
        }

        public void SetDescription(string? description)
        {
            Description = description?.Trim();
        }

        public void SetBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new DomainException("Thương hiệu không hợp lệ.");

            Brand = brand.Trim();
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

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

        public void AddImage(string url, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new DomainException("URL ảnh không hợp lệ.");

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new DomainException("URL ảnh không hợp lệ.");

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

        public void AddVariant(
            string sku,
            decimal price,
            decimal? originalPrice,
            string color,
            int initialStock,
            string? imageUrl = null,
            string? size = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new DomainException("SKU là bắt buộc.");

            if (price <= 0)
                throw new DomainException("Giá phải lớn hơn 0.");

            if (string.IsNullOrWhiteSpace(color))
                throw new DomainException("Màu sắc là bắt buộc.");

            if (_variants.Any(x => x.Sku == sku))
                throw new DomainException("SKU đã tồn tại.");

            var variant = new ProductVariant(
                Id,
                sku,
                price,
                originalPrice,
                color,
                size,
                imageUrl
            );

            _variants.Add(variant);

            AddDomainEvent(new VariantCreatedEvent(variant.Id, initialStock));
        }

        private static string GenerateSlug(string name)
        {
            string str = name.ToLowerInvariant();

            str = str.Normalize(NormalizationForm.FormD);
            var chars = str.Where(c =>
                System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                != System.Globalization.UnicodeCategory.NonSpacingMark);

            str = new string(chars.ToArray());

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-").Trim('-');

            return str;
        }
    }
}