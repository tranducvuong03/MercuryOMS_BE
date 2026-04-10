using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Application.Models;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetInventoryQuery(Guid VariantId) : IRequest<Result<InventoryResponse>>;

    public class GetInventoryHandler : IRequestHandler<GetInventoryQuery, Result<InventoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetInventoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<InventoryResponse>> Handle(GetInventoryQuery request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<Inventory>();

            var inventory = await repo.Query
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.VariantId == request.VariantId, ct);

            if (inventory == null)
                return Result<InventoryResponse>.Failure(Message.InventoryNotFound);

            var dto = new InventoryResponse
            {
                Available = inventory.Available,
                Reserved = inventory.Reserved
            };

            return Result<InventoryResponse>.Success(dto);
        }
    }
}
