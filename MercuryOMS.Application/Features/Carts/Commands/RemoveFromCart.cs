using MediatR;
using Microsoft.EntityFrameworkCore;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Features
{
    public record RemoveFromCartCommand(
        Guid ProductId,
        Guid VariantId
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

            if (userId == null)
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var cart = await _unitOfWork.GetRepository<Cart>().Query
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);

            if (cart is null)
                return Result.Failure(Message.CartItemNotFound);

            try
            {
                cart.RemoveItem(request.ProductId, request.VariantId);
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(Message.CartRemoveSuccessfully);
        }
    }
}