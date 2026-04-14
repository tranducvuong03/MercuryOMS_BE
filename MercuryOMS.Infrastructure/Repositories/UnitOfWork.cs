using MediatR;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Commons;
using MercuryOMS.Infrastructure.Data;

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

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, 
                                                bool dispatchEvents = true)
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            if (dispatchEvents)
                await DispatchDomainEvents(cancellationToken);

            return result;
        }

        private async Task DispatchDomainEvents(CancellationToken ct)
        {
            var domainEntities = _dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, ct);
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
