using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record DeleteProductCommand(Guid ProductId) : IRequest<Result>;

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
        {
            var product = await _unitOfWork.GetRepository<Product>()
                                            .GetByIdAsync(request.ProductId, ct);
            if (product == null)
                return Result.Failure(Message.ProductNotFound);

            _unitOfWork.GetRepository<Product>().Remove(product);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.ProductDeleted);
        }
    }
}
