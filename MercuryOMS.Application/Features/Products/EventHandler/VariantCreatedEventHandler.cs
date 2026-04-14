using MediatR;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Events;
using System.Text.Json;

namespace MercuryOMS.Application.Features
{
    public class VariantCreatedEventHandler
    : INotificationHandler<VariantCreatedEvent>
    {
        private readonly IUnitOfWork _uow;

        public VariantCreatedEventHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(VariantCreatedEvent e, CancellationToken ct)
        {
            var message = new InventoryInitMessage
            {
                VariantId = e.VariantId,
                InitialQuantity = e.InitialStock
            };

            var outbox = new OutboxMessage(
                type: nameof(InventoryInitMessage),
                queue: QueueNames.InventoryCreated,
                payload: JsonSerializer.Serialize(message)
            );

            await _uow.GetRepository<OutboxMessage>().AddAsync(outbox);
            await _uow.SaveChangesAsync();
        }
    }
}
