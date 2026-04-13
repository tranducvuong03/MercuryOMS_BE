using MercuryOMS.Application.IRepository;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _db;

        public InventoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Inventory?> GetByVariantIdAsync(Guid variantId)
        {
            return await _db.Set<Inventory>()
                .FirstOrDefaultAsync(x => x.VariantId == variantId);
        }
    }
}
