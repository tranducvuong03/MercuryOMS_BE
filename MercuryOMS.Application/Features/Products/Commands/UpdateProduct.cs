using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record UpdateProductCommand(
        Guid ProductId,
        string? Name = null,
        string? Description = null,
        bool? IsActive = null
    ) : IRequest<Result>;

    public class UpdateProductCommandHandler
        : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var productRepo = _unitOfWork.GetRepository<Product>();

            var product = await productRepo.GetByIdAsync(request.ProductId, ct);

            if (product == null)
                return Result.Failure(Message.ProductNotFound);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                product.SetName(request.Name);
                product.SetSlug(request.Name);
            }

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
