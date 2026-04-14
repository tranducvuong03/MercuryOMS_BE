using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.Models;
using MercuryOMS.Application.Models.Requests;
using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MercuryOMS.Application.Features
{
    public record GetProducts(ProductFilterRequest Filter)
    : IRequest<Result<BasePaginated<ProductResponse>>>;

    public class GetProductsHandler
    : IRequestHandler<GetProducts, Result<BasePaginated<ProductResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public GetProductsHandler(
            IUnitOfWork unitOfWork,
            ICacheService cache)
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
                {
                    return Result<BasePaginated<ProductResponse>>
                        .Success(cached, Message.ProductGetFromCache);
                }

                var result = await QueryPagedAsync(f, ct);

                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                return Result<BasePaginated<ProductResponse>>
                    .Success(result, Message.ProductGetSuccess);
            }
            catch (Exception)
            {
                return Result<BasePaginated<ProductResponse>>
                    .Failure(Message.ProductGetFailed);
            }
        }

        private List<Expression<Func<Product, bool>>> BuildFilters(ProductFilterRequest f)
        {
            var filters = new List<Expression<Func<Product, bool>>>();

            if (f.IsActive)
                filters.Add(x => x.IsActive);

            if (f.MinPrice.HasValue)
                filters.Add(x => x.BasePrice >= f.MinPrice.Value);

            if (f.MaxPrice.HasValue)
                filters.Add(x => x.BasePrice <= f.MaxPrice.Value);

            return filters;
        }

        private async Task<BasePaginated<ProductResponse>> QueryPagedAsync(
            ProductFilterRequest f, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var filters = BuildFilters(f);

            var query = repo.GetByFilterWithPaginated(f.PageIndex, f.PageSize, filters);

            var items = await query
                .OrderBy(x => x.Name)
                .Select(x => new ProductResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    BasePrice = x.BasePrice,
                    IsActive = x.IsActive,

                    ThumbnailUrl = x.Images
                        .Where(i => i.IsPrimary)
                        .Select(i => i.Url)
                        .FirstOrDefault()
                })
                .ToListAsync(ct);

            var totalItems = await (await repo.GetByFiltersAsync(filters)).CountAsync(ct);

            return new BasePaginated<ProductResponse>(items, f.PageIndex, f.PageSize, totalItems);
        }
    }
}