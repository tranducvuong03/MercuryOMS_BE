using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record GetProductByIdQuery(Guid ProductId) : IRequest<Result<Product>>;

    public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<Product>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Product>> Handle(GetProductByIdQuery request, CancellationToken ct)
        {
            var product = await _unitOfWork
                .GetRepository<Product>()
                .GetByIdAsync(request.ProductId, ct);

            if (product == null)
                return Result<Product>.Failure(Message.ProductNotFound);

            return Result<Product>.Success(product);
        }
    }
}
