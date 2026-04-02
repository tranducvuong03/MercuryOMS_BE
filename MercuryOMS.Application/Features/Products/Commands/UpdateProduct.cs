using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record UpdateProductCommand(
        Guid ProductId,
        string? Name = null,
        decimal? BasePrice = null,
        string? Description = null,
        bool? IsActive = null
    ) : IRequest<Result>;

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var product = await _unitOfWork.GetRepository<Product>()
                                            .GetByIdAsync(request.ProductId, ct);
            if (product == null)
                return Result.Failure(Message.ProductNotFound);

            if (!string.IsNullOrWhiteSpace(request.Name))
                product.SetName(request.Name);

            if (request.BasePrice.HasValue)
                product.SetBasePrice(request.BasePrice.Value);

            if (request.Description != null)
                product.SetDescription(request.Description);

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                    product.Activate();
                else
                    product.Deactivate();
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.ProductUpdated);
        }
    }
}
