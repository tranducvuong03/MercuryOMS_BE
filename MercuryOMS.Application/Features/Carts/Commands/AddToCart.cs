using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record AddToCartCommand(
        Guid ProductId,
        Guid VariantId,
        int Quantity
    ) : IRequest<Result>;

    public class AddToCartHandler : IRequestHandler<AddToCartCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public AddToCartHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(AddToCartCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var cartRepo = _unitOfWork.GetRepository<Cart>();
            var inventoryRepo = _unitOfWork.GetRepository<Inventory>();
            var variantRepo = _unitOfWork.GetRepository<ProductVariant>();

            var cart = await cartRepo.Query
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart == null)
            {
                cart = new Cart(userId.Value);
                await cartRepo.AddAsync(cart, ct);
            }

            var variant = await variantRepo.Query
                .FirstOrDefaultAsync(x => x.Id == request.VariantId, ct);

            if (variant == null)
                return Result.Failure("Variant không tồn tại");

            if (variant.ProductId != request.ProductId)
                return Result.Failure("Variant không thuộc product");

            var inventory = await inventoryRepo.Query
                .FirstOrDefaultAsync(x => x.VariantId == request.VariantId, ct);

            if (inventory == null)
                return Result.Failure("Không tìm thấy tồn kho");

            var existingItem = cart.Items
                .FirstOrDefault(i =>
                    i.ProductId == request.ProductId &&
                    i.VariantId == request.VariantId);

            var currentQty = existingItem?.Quantity ?? 0;
            var newQty = currentQty + request.Quantity;

            if (newQty > inventory.Available)
                return Result.Failure("Số lượng vượt quá tồn kho");

            cart.AddItem(
                request.ProductId,
                request.VariantId,
                request.Quantity
            );

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.CartAddSuccessfully);
        }
    }
}