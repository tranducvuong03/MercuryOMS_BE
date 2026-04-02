
using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record ReleaseInventoryCommand(
        Guid ProductId,
        int Quantity,
        Guid ReferenceId
    ) : IRequest<Result>;

    public class ReleaseInventoryHandler : IRequestHandler<ReleaseInventoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReleaseInventoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ReleaseInventoryCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<Inventory>();

            var inventory = await repo.Query
                .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, ct);

            if (inventory == null)
                return Result.Failure(Message.InventoryNotFound);

            inventory.Release(request.Quantity, request.ReferenceId);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.InventoryReleased);
        }
    }
}
