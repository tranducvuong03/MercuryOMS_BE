using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record RemoveFromCartCommand(
        Guid ProductId
    ) : IRequest<Result>;

    public class RemoveFromCartHandler : IRequestHandler<RemoveFromCartCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public RemoveFromCartHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            var cart = await _unitOfWork.GetRepository<Cart>().Query
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart is null)
                return Result.Failure(Message.CartItemNotFound);

            try
            {
                cart.RemoveItem(request.ProductId);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.CartRemoveSuccessfully);
        }
    }
}
