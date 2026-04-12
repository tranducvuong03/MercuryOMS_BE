using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record AdjustInventoryCommand(
        Guid VariantId,
        int Quantity,
        Guid? ReferenceId
    ) : IRequest<Result>;

    public class AdjustInventoryHandler : IRequestHandler<AdjustInventoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdjustInventoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AdjustInventoryCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<Inventory>();

            var inventory = await repo.Query
                .FirstOrDefaultAsync(x => x.VariantId == request.VariantId, ct);

            if (inventory == null)
            {
                inventory = new Inventory(request.VariantId, 0);
                await repo.AddAsync(inventory, ct);
            }

            inventory.Adjust(request.Quantity, request.ReferenceId);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.InventoryAdjusted);
        }
    }
}
