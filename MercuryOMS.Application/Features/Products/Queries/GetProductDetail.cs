using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetProductDetail(Guid ProductId)
        : IRequest<Result<ProductDetailResponse>>;

    public class GetProductDetailHandler
        : IRequestHandler<GetProductDetail, Result<ProductDetailResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetProductDetailHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<ProductDetailResponse>> Handle(
            GetProductDetail request,
            CancellationToken ct)
        {
            try
            {
                var cacheKey = CacheKeys.ProductDetail(request.ProductId);

                var cached = await _cache.GetAsync<ProductDetailResponse>(cacheKey);
                if (cached != null)
                {
                    return Result<ProductDetailResponse>
                        .Success(cached, Message.ProductGetFromCache);
                }

                var productRepo = _unitOfWork.GetRepository<Product>();

                var product = await productRepo.Query
                    .Include(x => x.Images)
                    .Include(x => x.Variants)
                    .Include(x => x.Categories)
                    .FirstOrDefaultAsync(x => x.Id == request.ProductId, ct);

                if (product == null)
                    return Result<ProductDetailResponse>
                        .Failure(Message.ProductNotFound);

                // lấy review
                var reviewStats = await _unitOfWork.GetRepository<Review>().Query
                    .Join(
                        _unitOfWork.GetRepository<OrderItem>().Query,
                        r => r.OrderItemId,
                        oi => oi.Id,
                        (r, oi) => new { r, oi }
                    )
                    .Where(x => x.oi.ProductId == product.Id)
                    .GroupBy(x => x.oi.ProductId)
                    .Select(g => new
                    {
                        Avg = g.Average(x => (double?)x.r.Rating) ?? 0,
                        Count = g.Count()
                    })
                    .FirstOrDefaultAsync(ct);

                // lấy số lượng đã bán
                var sold = await _unitOfWork.GetRepository<OrderItem>().Query
                    .Join(
                        _unitOfWork.GetRepository<Order>().Query,
                        oi => oi.OrderId,
                        o => o.Id,
                        (oi, o) => new { oi, o }
                    )
                    .Where(x =>
                        x.oi.ProductId == product.Id &&
                        x.o.Status == OrderStatus.Completed)
                    .SumAsync(x => (int?)x.oi.Quantity, ct) ?? 0;

                var categoryIds = product.Categories
                    .Select(x => x.CategoryId)
                    .ToList();

                var categories = await _unitOfWork.GetRepository<Category>().Query
                    .Where(c => categoryIds.Contains(c.Id))
                    .Select(c => c.Name)
                    .ToListAsync(ct);

                var category = categories.FirstOrDefault();

                var variantIds = product.Variants.Select(v => v.Id).ToList();

                var inventories = await _unitOfWork.GetRepository<Inventory>().Query
                    .Where(x => variantIds.Contains(x.VariantId))
                    .ToListAsync(ct);

                var inventoryDict = inventories.ToDictionary(x => x.VariantId);

                // lấy giá gốc thấp nhất và giá giảm cao nhất 
                decimal originalPrice = product.Variants.Min(v => v.OriginalPrice);
                decimal? discountPrice = product.Variants
                    .Where(v => v.DiscountPrice.HasValue)
                    .Select(v => v.DiscountPrice)
                    .Min();

                var variants = product.Variants.Select(v =>
                {
                    inventoryDict.TryGetValue(v.Id, out var inv);

                    var hasDiscount = v.DiscountPrice.HasValue && v.DiscountPrice < v.OriginalPrice;

                    return new ProductVariantResponse
                    {
                        Id = v.Id,
                        Color = v.Color,
                        Size = v.Size,
                        ImageUrl = v.ImageUrl,

                        OriginalPrice = hasDiscount ? v.OriginalPrice : v.OriginalPrice,
                        DiscountPrice = hasDiscount ? v.DiscountPrice : null,

                        Stock = inv?.Available ?? 0
                    };
                }).ToList();

                decimal? discountPercent = null;

                var discountedVariants = product.Variants
                    .Where(v => v.DiscountPrice.HasValue && v.DiscountPrice < v.OriginalPrice)
                    .ToList();

                if (discountedVariants.Any())
                {
                    var maxOriginal = discountedVariants.Max(v => v.OriginalPrice);
                    var minDiscount = discountedVariants.Min(v => v.DiscountPrice!.Value);

                    discountPercent = Math.Round(
                        (maxOriginal - minDiscount) / maxOriginal * 100,
                        0
                    );
                }

                var result = new ProductDetailResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    IsActive = product.IsActive,

                    Category = category,

                    OriginalPrice = originalPrice,
                    DiscountPrice = discountPrice,

                    Images = product.Images
                        .OrderBy(x => x.IsPrimary ? 0 : 1)
                        .Select(x => x.Url)
                        .ToList(),

                    Variants = variants,

                    Seller = new SellerResponse
                    {
                        Id = product.CreatedBy,
                        Name = product.CreatedBy,
                        Avatar = null
                    },

                    Rating = reviewStats?.Avg ?? 0,
                    ReviewCount = reviewStats?.Count ?? 0,
                    Sold = sold,

                    Discount = discountPercent
                };

                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return Result<ProductDetailResponse>
                    .Success(result, Message.ProductGetSuccess);
            }
            catch
            {
                return Result<ProductDetailResponse>
                    .Failure(Message.ProductGetFailed);
            }
        }
    }
}