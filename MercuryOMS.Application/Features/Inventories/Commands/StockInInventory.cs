using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record StockInInventoryCommand(
        Guid VariantId,
        int Quantity,
        Guid? ReferenceId
    ) : IRequest<Result>;

    public class StockInInventoryHandler : IRequestHandler<StockInInventoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockInInventoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(StockInInventoryCommand request, CancellationToken ct)
        {
            if (request.Quantity <= 0)
                return Result.Failure("Quantity phải > 0");

            var repo = _unitOfWork.GetRepository<Inventory>();

            var inventory = await repo.Query
                .FirstOrDefaultAsync(x => x.VariantId == request.VariantId, ct);

            if (inventory == null)
            {
                inventory = new Inventory(request.VariantId, 0);
                await repo.AddAsync(inventory, ct);
            }

            try
            {
                inventory.StockIn(request.Quantity, request.ReferenceId);
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.InventoryStockIn);
        }
    }
}
