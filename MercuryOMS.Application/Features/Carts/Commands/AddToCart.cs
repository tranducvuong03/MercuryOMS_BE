using MediatR;
using Microsoft.EntityFrameworkCore;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Constants;

namespace MercuryOMS.Application.Features
{
    public record AddToCartCommand(
        Guid ProductId,
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

            var cart = await _unitOfWork.GetRepository<Cart>().Query
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart is null)
            {
                cart = new Cart(userId);
                await _unitOfWork.GetRepository<Cart>().AddAsync(cart, ct);
            }

            cart.AddItem(request.ProductId, request.Quantity);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.CartAddSuccessfully);
        }
    }
}
