using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetProductImages(Guid ProductId)
    : IRequest<Result<List<ProductImageResponse>>>;

    public class GetProductImagesHandler
    : IRequestHandler<GetProductImages, Result<List<ProductImageResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductImagesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ProductImageResponse>>> Handle(
            GetProductImages request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<Product>();

            var images = await repo.Query
                .Where(x => x.Id == request.ProductId)
                .SelectMany(x => x.Images)
                .Select(i => new ProductImageResponse
                {
                    Url = i.Url,
                    IsPrimary = i.IsPrimary
                })
                .ToListAsync(ct);

            return Result<List<ProductImageResponse>>.Success(images);
        }
    }
}
