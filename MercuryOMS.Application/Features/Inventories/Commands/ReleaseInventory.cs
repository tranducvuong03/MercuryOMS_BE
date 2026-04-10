using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record ReleaseInventoryCommand(
        Guid VariantId,
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
            if (request.VariantId == Guid.Empty)
                return Result.Failure("VariantId không hợp lệ");

            if (request.Quantity <= 0)
                return Result.Failure("Quantity phải > 0");

            var repo = _unitOfWork.GetRepository<Inventory>();

            var inventory = await repo.Query
                .FirstOrDefaultAsync(x => x.VariantId == request.VariantId, ct);

            if (inventory == null)
                return Result.Failure(Message.InventoryNotFound);

            try
            {
                inventory.Release(request.Quantity, request.ReferenceId);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.InventoryReleased);
        }
    }
}