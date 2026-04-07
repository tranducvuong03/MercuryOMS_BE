using MercuryOMS.Application.Interfaces;
using MercuryOMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MercuryOMS.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public IQueryable<T> Query => _db.Set<T>().AsQueryable();

        public async Task<T?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _db.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IQueryable<T>> GetByFiltersAsync(IEnumerable<Expression<Func<T, bool>>> filters)
        {
            IQueryable<T> query = _dbSet;
            if (filters is not null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }
            return query;
        }

        public IQueryable<T> GetByFilterWithPaginated(
            int pageIndex,
            int pageSize,
            IEnumerable<Expression<Func<T, bool>>>? filters = null)
        {
            IQueryable<T> query = _dbSet;

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            query = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return query;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _db.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public void Update(T entity)
        {
            _db.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _db.Set<T>().Remove(entity);
        }
    }
}
