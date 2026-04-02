using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record StockInInventoryCommand(
        Guid ProductId,
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
            var repo = _unitOfWork.GetRepository<Inventory>();

            var inventory = await repo.Query
                .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, ct);

            if (inventory == null)
                return Result.Failure(Message.InventoryNotFound);

            inventory.StockIn(request.Quantity, request.ReferenceId);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.InventoryStockIn);
        }
    }
}
