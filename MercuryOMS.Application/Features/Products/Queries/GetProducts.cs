using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.Models;
using MercuryOMS.Application.Models.Requests;
using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetProducts(ProductFilterRequest Filter)
        : IRequest<Result<BasePaginated<ProductResponse>>>;

    public class GetProductsHandler
        : IRequestHandler<GetProducts, Result<BasePaginated<ProductResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetProductsHandler(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<BasePaginated<ProductResponse>>> Handle(
            GetProducts request, CancellationToken ct)
        {
            try
            {
                var f = request.Filter;

                if (f == null)
                    return Result<BasePaginated<ProductResponse>>
                        .Failure(Message.ProductFilterInvalid);

                var cacheKey = CacheKeys.ProductsPaged(
                    f.PageIndex, f.PageSize,
                    f.IsActive, f.MinPrice, f.MaxPrice);

                var cached = await _cache.GetAsync<BasePaginated<ProductResponse>>(cacheKey);
                if (cached != null)
                    return Result<BasePaginated<ProductResponse>>
                        .Success(cached, Message.ProductGetFromCache);

                var productRepo = _unitOfWork.GetRepository<Product>();
                var variantRepo = _unitOfWork.GetRepository<ProductVariant>();

                var variantQuery = variantRepo.Query;

                if (f.MinPrice.HasValue)
                {
                    variantQuery = variantQuery.Where(v =>
                        (v.DiscountPrice ?? v.OriginalPrice) >= f.MinPrice.Value);
                }

                if (f.MaxPrice.HasValue)
                {
                    variantQuery = variantQuery.Where(v =>
                        (v.DiscountPrice ?? v.OriginalPrice) <= f.MaxPrice.Value);
                }

                var validProductIds = await variantQuery
                    .Select(v => v.ProductId)
                    .Distinct()
                    .ToListAsync(ct);

                var productQuery = productRepo.Query
                    .Where(p => validProductIds.Contains(p.Id));

                if (f.IsActive)
                    productQuery = productQuery.Where(p => p.IsActive);

                productQuery = productQuery.OrderBy(p => p.Name);

                var totalItems = await productQuery.CountAsync(ct);

                var products = await productQuery
                    .Skip((f.PageIndex - 1) * f.PageSize)
                    .Take(f.PageSize)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.IsActive,
                        Thumbnail = p.Images
                            .Where(i => i.IsPrimary)
                            .Select(i => i.Url)
                            .FirstOrDefault()
                    })
                    .ToListAsync(ct);

                var productIds = products.Select(x => x.Id).ToList();

                // lấy review
                var reviewStats = await _unitOfWork.GetRepository<Review>().Query
                    .Join(
                        _unitOfWork.GetRepository<OrderItem>().Query,
                        r => r.OrderItemId,
                        oi => oi.Id,
                        (r, oi) => new { r, oi }
                    )
                    .Where(x => productIds.Contains(x.oi.ProductId))
                    .GroupBy(x => x.oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Avg = g.Average(x => (double?)x.r.Rating) ?? 0,
                        Count = g.Count()
                    })
                    .ToListAsync(ct);

                // lấy số lượng đã bán
                var soldStats = await _unitOfWork.GetRepository<OrderItem>().Query
                    .Join(
                        _unitOfWork.GetRepository<Order>().Query,
                        oi => oi.OrderId,
                        o => o.Id,
                        (oi, o) => new { oi, o }
                    )
                    .Where(x =>
                        productIds.Contains(x.oi.ProductId) &&
                        x.o.Status == Domain.Enums.OrderStatus.Completed)
                    .GroupBy(x => x.oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Sold = g.Sum(x => x.oi.Quantity)
                    })
                    .ToListAsync(ct);

                var variantGroups = await variantRepo.Query
                    .Where(v => productIds.Contains(v.ProductId))
                    .GroupBy(v => v.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,

                        OriginalPrice = g.Max(v => v.OriginalPrice),

                        EffectiveMinPrice = g.Min(v => v.DiscountPrice ?? v.OriginalPrice),

                        HasDiscount = g.Any(v => v.DiscountPrice != null)
                    })
                    .ToListAsync(ct);

                var reviewDict = reviewStats.ToDictionary(x => x.ProductId);
                var soldDict = soldStats.ToDictionary(x => x.ProductId);
                var priceDict = variantGroups.ToDictionary(x => x.ProductId);

                var items = products.Select(p =>
                {
                    reviewDict.TryGetValue(p.Id, out var r);
                    soldDict.TryGetValue(p.Id, out var s);
                    priceDict.TryGetValue(p.Id, out var price);

                    decimal originalPrice = price?.OriginalPrice ?? 0;

                    decimal? discountPrice = price != null && price.HasDiscount
                                            ? price.EffectiveMinPrice
                                            : null;

                    decimal? discount =
                        (price != null &&
                         price.HasDiscount &&
                         originalPrice > 0 &&
                         discountPrice.HasValue)
                            ? Math.Round((originalPrice - discountPrice.Value) / originalPrice * 100, 0)
                            : null;

                    return new ProductResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        IsActive = p.IsActive,
                        ThumbnailUrl = p.Thumbnail,

                        OriginalPrice = originalPrice,
                        DiscountPrice = discountPrice,

                        Sold = s?.Sold ?? 0,
                        Rating = r?.Avg ?? 0,
                        ReviewCount = r?.Count ?? 0,

                        Badge = !p.IsActive
                            ? "KHÔNG HOẠT ĐỘNG"
                            : (discount.HasValue ? "GIẢM GIÁ" : null),

                        Discount = discount
                    };
                }).ToList();

                var paginated = new BasePaginated<ProductResponse>(
                    items,
                    f.PageIndex,
                    f.PageSize,
                    totalItems
                );

                return Result<BasePaginated<ProductResponse>>.Success(paginated);
            }
            catch
            {
                return Result<BasePaginated<ProductResponse>>
                    .Failure(Message.ProductGetFailed);
            }
        }
    }
}