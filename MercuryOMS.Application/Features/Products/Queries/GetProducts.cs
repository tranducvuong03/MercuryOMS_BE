using MediatR;
using MercuryOMS.Application.Common;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Entities;
using System.Linq.Expressions;

namespace MercuryOMS.Application.Features
{
    public record ListProductsPaginatedQuery(
        int PageIndex = 1,
        int PageSize = 10,
        bool OnlyActive = true
    ) : IRequest<Result<BasePaginated<Product>>>;

    public class ListProductsPaginatedQueryHandler
    : IRequestHandler<ListProductsPaginatedQuery, Result<BasePaginated<Product>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListProductsPaginatedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<BasePaginated<Product>>> Handle(
            ListProductsPaginatedQuery request,
            CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<Product>();

            var filters = new List<Expression<Func<Product, bool>>>();
            if (request.OnlyActive)
                filters.Add(x => x.IsActive);

            var query = repo.GetByFilterWithPaginated(
                request.PageIndex,
                request.PageSize,
                filters);

            var items = query.ToList();

            var totalItems = repo.Query
                .Where(x => !request.OnlyActive || x.IsActive)
                .Count();

            var result = new BasePaginated<Product>(
                items,
                request.PageIndex,
                request.PageSize,
                totalItems);

            return Result<BasePaginated<Product>>.Success(result);
        }
    }
}
