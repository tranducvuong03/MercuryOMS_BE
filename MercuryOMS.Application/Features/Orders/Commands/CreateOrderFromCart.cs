using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record CreateOrderFromCartCommand() : IRequest<Result<Guid>>;

    public class CreateOrderFromCartHandler
        : IRequestHandler<CreateOrderFromCartCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public CreateOrderFromCartHandler(
            IUnitOfWork uow,
            ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<Guid>> Handle(
            CreateOrderFromCartCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                return Result<Guid>.Failure(Message.OrderUserInvalid);

            var cart = await _uow.GetRepository<Cart>().Query
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart == null || !cart.Items.Any())
                return Result<Guid>.Failure("Giỏ hàng trống.");

            // lấy địa chỉ mặc định của user
            var address = await _uow.GetRepository<UserAddress>().Query
                .Where(x => x.UserId == userId && x.IsDefault)
                .Select(x => x.Address)
                .FirstOrDefaultAsync(ct);

            if (address == null)
                return Result<Guid>.Failure("Chưa có địa chỉ mặc định.");

            var variantIds = cart.Items
                .Select(i => i.VariantId)
                .ToList();

            var variants = await _uow.GetRepository<ProductVariant>()
                .Query
                .Where(v => variantIds.Contains(v.Id))
                .ToDictionaryAsync(v => v.Id, ct);

            var inventories = await _uow.GetRepository<Inventory>()
                .Query
                .Where(i => variantIds.Contains(i.VariantId))
                .ToDictionaryAsync(i => i.VariantId, ct);

            var order = new Order(userId.Value, address);

            foreach (var item in cart.Items)
            {
                if (!variants.ContainsKey(item.VariantId))
                    return Result<Guid>.Failure("Phân loại không tồn tại.");

                if (!inventories.ContainsKey(item.VariantId))
                    return Result<Guid>.Failure("Không tìm thấy tồn kho.");

                if (item.Quantity > inventories[item.VariantId].Available)
                    return Result<Guid>.Failure("Số lượng vượt quá tồn kho.");
            }

            foreach (var item in cart.Items)
            {
                var variant = variants[item.VariantId];
                var inventory = inventories[item.VariantId];

                var price = variant.DiscountPrice ?? variant.OriginalPrice;

                order.AddItem(
                    variant.ProductId,
                    variant.Id,
                    item.Quantity,
                    price
                );

                inventory.Reserve(item.Quantity, order.Id);
            }

            await _uow.GetRepository<Order>().AddAsync(order, ct);

            cart.Clear();

            await _uow.SaveChangesAsync(ct);

            return Result<Guid>.Success(order.Id, Message.OrderCreated);
        }
    }
}