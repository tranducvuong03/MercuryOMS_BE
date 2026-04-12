using AutoMapper;
using MediatR;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.Models;

namespace MercuryOMS.Application.Features
{
    public record GetByIdQuery(Guid Id) : IRequest<CategoryResponse>;

    public class GetByIdHandler : IRequestHandler<GetByIdQuery, CategoryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.GetRepository<Domain.Entities.Category>().GetByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<CategoryResponse>(category);
        }
    }
}
