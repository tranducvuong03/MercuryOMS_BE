using AutoMapper;
using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.Models;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record GetAllCategoryQuery()
        : IRequest<Result<List<CategoryResponse>>>;

    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, Result<List<CategoryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoryQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryResponse>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var categories = _unitOfWork.GetRepository<Category>().Query.ToList();
            var categoryResponses = _mapper.Map<List<CategoryResponse>>(categories);
            return Result<List<CategoryResponse>>.Success(categoryResponses);
        }
    }
}