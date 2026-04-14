using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetProductVariants(Guid ProductId)
        : IRequest<Result<List<ProductVariantResponse>>>;

    public class GetProductVariantsHandler
        : IRequestHandler<GetProductVariants, Result<List<ProductVariantResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductVariantsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ProductVariantResponse>>> Handle(
            GetProductVariants request, CancellationToken ct)
        {
            var variantRepo = _unitOfWork.GetRepository<ProductVariant>();
            var inventoryRepo = _unitOfWork.GetRepository<Inventory>();

            var variants = await (
                from v in variantRepo.Query
                join i in inventoryRepo.Query
                    on v.Id equals i.VariantId into invGroup
                from i in invGroup.DefaultIfEmpty()
                where v.ProductId == request.ProductId
                select new ProductVariantResponse
                {
                    Id = v.Id,
                    Sku = v.Sku,
                    Price = v.Price,
                    Stock = i != null ? i.Available : 0
                }
            ).ToListAsync(ct);

            return Result<List<ProductVariantResponse>>
                .Success(variants);
        }
    }
}