using MediatR;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Commons;
using MercuryOMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly Dictionary<Type, object> _repositories = new();
        private readonly IMediator _mediator;

        public UnitOfWork(ApplicationDbContext context, IMediator mediator)
        {
            _dbContext = context;
            _mediator = mediator;
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var repo = new GenericRepository<T>(_dbContext);
                _repositories[type] = repo;
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            await DispatchDomainEvents();

            return result;
        }

        private async Task DispatchDomainEvents()
        {
            var entities = _dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            var events = entities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // clear trước khi chạy tránh duplicate
            entities.ForEach(e => e.Entity.ClearDomainEvents());

            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent);
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
