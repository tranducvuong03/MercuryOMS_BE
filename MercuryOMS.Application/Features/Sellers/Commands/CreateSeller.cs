using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record CreateSellerCommand(string Name) : IRequest<Result<Guid>>;

    public class CreateSellerCommandHandler
        : IRequestHandler<CreateSellerCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public CreateSellerCommandHandler(
            IUnitOfWork uow,
            ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<Guid>> Handle(
            CreateSellerCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId.ToString();

            if (userId == string.Empty) return Result<Guid>.Failure("Chưa đăng nhập.");

            var sellerRepo = _uow.GetRepository<Seller>();

            var exists = await sellerRepo.Query
                .AnyAsync(x => x.UserId == userId, ct);

            if (exists)
                return Result<Guid>.Failure("Đã là seller rồi.");

            var seller = new Seller(userId, request.Name);

            await sellerRepo.AddAsync(seller, ct);

            await _uow.SaveChangesAsync(ct);

            return Result<Guid>.Success(seller.Id);
        }
    }
}
