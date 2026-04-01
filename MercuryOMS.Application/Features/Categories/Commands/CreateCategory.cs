using AutoMapper;
using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record CreateCategoryCommand(
        string Name,
        string Description
    ) : IRequest<Result<CategoryResponse>>;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            Category category = new Category(request.Name, request.Description);

            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var res = _mapper.Map<CategoryResponse>(category);

            return Result<CategoryResponse>.Success(res);
        }
    }
}