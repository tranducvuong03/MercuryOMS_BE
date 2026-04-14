using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetProductCategories(Guid ProductId)
    : IRequest<Result<List<ProductCategoryResponse>>>;

    public class GetProductCategoriesHandler
    : IRequestHandler<GetProductCategories, Result<List<ProductCategoryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductCategoriesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ProductCategoryResponse>>> Handle(
            GetProductCategories request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<Product>();

            var categories = await repo.Query
                .Where(x => x.Id == request.ProductId)
                .SelectMany(x => x.Categories)
                .Select(c => new ProductCategoryResponse
                {
                    CategoryId = c.CategoryId
                })
                .ToListAsync(ct);

            return Result<List<ProductCategoryResponse>>.Success(categories);
        }
    }
}
