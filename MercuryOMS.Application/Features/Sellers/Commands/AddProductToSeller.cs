
using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record AddProductToSellerCommand(
        Guid ProductId,
        decimal CommissionRate
    ) : IRequest<Result>;

    public class AddProductToSellerCommandHandler
    : IRequestHandler<AddProductToSellerCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public AddProductToSellerCommandHandler(
            IUnitOfWork uow,
            ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(
            AddProductToSellerCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId.ToString();

            var sellerRepo = _uow.GetRepository<Seller>();

            var seller = await sellerRepo.Query
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);

            if (seller == null)
                return Result.Failure("Bạn chưa là seller.");

            var productRepo = _uow.GetRepository<Product>();

            var productExists = await productRepo.Query
                .AnyAsync(x => x.Id == request.ProductId, ct);

            if (!productExists)
                return Result.Failure(Message.ProductNotFound);

            seller.AddProduct(request.ProductId, request.CommissionRate);

            await _uow.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
