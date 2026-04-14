using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Worker.OutboxProcessor
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                var messages = await db.Set<OutboxMessage>()
                    .Where(x => !x.IsProcessed)
                    .OrderBy(x => x.OccurredOn)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var msg in messages)
                {
                    try
                    {
                        await bus.PublishAsync(msg.Queue, msg.Payload);

                        msg.MarkAsProcessed();
                        msg.ProcessedOn = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Outbox error: {ex.Message}");
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}