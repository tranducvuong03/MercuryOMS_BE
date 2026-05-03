using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record GetCartQuery() : IRequest<Result<CartResponse>>;

    public class GetCartHandler : IRequestHandler<GetCartQuery, Result<CartResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetCartHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<CartResponse>> Handle(GetCartQuery request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var cartRepo = _unitOfWork.GetRepository<Cart>();
            var productRepo = _unitOfWork.GetRepository<Product>();
            var variantRepo = _unitOfWork.GetRepository<ProductVariant>();
            var inventoryRepo = _unitOfWork.GetRepository<Inventory>();

            var cart = await cartRepo.Query
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);

            if (cart == null || !cart.Items.Any())
                return Result<CartResponse>.Success(new CartResponse());

            var productIds = cart.Items.Select(x => x.ProductId).Distinct().ToList();
            var variantIds = cart.Items.Select(x => x.VariantId).Distinct().ToList();

            var products = await productRepo.Query
                .Where(x => productIds.Contains(x.Id))
                .ToListAsync(ct);

            var variants = await variantRepo.Query
                .Where(x => variantIds.Contains(x.Id))
                .ToListAsync(ct);

            var inventories = await inventoryRepo.Query
                .Where(x => variantIds.Contains(x.VariantId))
                .ToListAsync(ct);

            var items = (from item in cart.Items
                         join p in products on item.ProductId equals p.Id
                         join v in variants on item.VariantId equals v.Id
                         join i in inventories on item.VariantId equals i.VariantId into inv
                         from stock in inv.DefaultIfEmpty()
                         select new CartItemResponse
                         {
                             ProductId = item.ProductId,
                             VariantId = item.VariantId,

                             ProductName = p.Name,
                             Image = v.ImageUrl ?? p.Thumbnail,

                             Color = v.Color,
                             Size = v.Size,

                             Price = v.OriginalPrice,
                             DiscountPrice = v.DiscountPrice,

                             Quantity = item.Quantity,
                             Stock = stock?.Available ?? 0
                         }).ToList();

            return Result<CartResponse>.Success(new CartResponse
            {
                Items = items,
                TotalPrice = items.Sum(x => x.Total)
            }, "Lấy giỏ thành công");
        }
    }
}
