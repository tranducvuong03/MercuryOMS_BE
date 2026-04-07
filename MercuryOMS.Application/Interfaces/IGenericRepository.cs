using System.Linq.Expressions;

namespace MercuryOMS.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Query { get; }
        Task<IQueryable<T>> GetByFiltersAsync(
            IEnumerable<Expression<Func<T, bool>>> filters);
        IQueryable<T> GetByFilterWithPaginated(
            int pageIndex,
            int pageSize,
            IEnumerable<Expression<Func<T, bool>>>? filters = null);
        Task<T?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<T> AddAsync(
            T entity,
            CancellationToken cancellationToken = default);
        void Update(T entity);
        void Remove(T entity);
    }
}
