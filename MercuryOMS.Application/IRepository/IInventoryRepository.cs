using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.IRepository
{
    public interface IInventoryRepository
    {
        Task<Inventory?> GetByVariantIdAsync(Guid variantId);
    }
}
