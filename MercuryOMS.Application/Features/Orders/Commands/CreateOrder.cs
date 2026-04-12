using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.Models.Requests;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record CreateOrderCommand(
       List<CreateOrderItemRequest> Items
    ) : IRequest<Result<Guid>>;

    public class CreateOrderHandler
        : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateOrderHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result<Guid>> Handle(
            CreateOrderCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            try
            {
                if (userId == Guid.Empty)
                    return Result<Guid>.Failure(Message.OrderUserInvalid);

                var productIds = request.Items.Select(x => x.ProductId).ToList();

                var products = await _unitOfWork.GetRepository<Product>().Query
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, ct);

                if (products.Count != productIds.Count)
                    return Result<Guid>.Failure(Message.OrderProductNotFound);

                var order = new Order(userId);

                foreach (var item in request.Items)
                {
                    var product = products[item.ProductId];

                    if (!product.IsActive)
                        return Result<Guid>.Failure(
                            $"{Message.OrderProductInactive}: {product.Name}"
                        );

                    var price = product.BasePrice;

                    order.AddItem(
                        item.ProductId,
                        item.Quantity,
                        price
                    );
                }

                await _unitOfWork.GetRepository<Order>().AddAsync(order, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                return Result<Guid>.Success(order.Id, Message.OrderCreated);
            }
            catch
            {
                return Result<Guid>.Failure(Message.OrderCreateFailed);
            }
        }
    }
}