using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Worker
{
    public class InventoryInitConsumer
    : RabbitMqConsumerBase<InventoryInitMessage>
    {
        protected override string QueueName => "inventory.created";

        public InventoryInitConsumer(IServiceScopeFactory scopeFactory)
            : base(scopeFactory)
        {
        }

        protected override async Task HandleMessageAsync(
            IServiceScope scope,
            InventoryInitMessage message,
            CancellationToken ct)
        {
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var repo = uow.GetRepository<Inventory>();

            var exists = await repo.Query
                .AnyAsync(x => x.VariantId == message.VariantId, ct);

            if (exists) return;

            var inventory = new Inventory(
                message.VariantId,
                message.InitialQuantity
            );

            await repo.AddAsync(inventory);
            await uow.SaveChangesAsync( ct, false);
        }
    }
}
