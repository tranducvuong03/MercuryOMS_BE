using Ganss.Xss;
using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record CreateReviewCommand(
        Guid OrderItemId,
        int Rating,
        string? Comment,
        List<string>? ImageUrls
    ) : IRequest<Result>;

    public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateReviewHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(CreateReviewCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                throw new UnauthorizedAccessException();

            var orderRepo = _unitOfWork.GetRepository<Order>();
            var reviewRepo = _unitOfWork.GetRepository<Review>();

            var order = await orderRepo.Query
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Items
                .Any(i => i.Id == request.OrderItemId), ct);

            if (order == null)
                throw new DomainException("Không tìm thấy đơn hàng");

            if (order.UserId != userId)
                throw new DomainException("Không phải đơn hàng của bạn");

            if (order.Status < OrderStatus.Completed)
                throw new DomainException("Chưa đủ điều kiện đánh giá");

            var item = order.Items.First(x => x.Id == request.OrderItemId);

            item.MarkAsReviewed();

            string? cleanComment = null;
            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedTags.Clear();

                cleanComment = sanitizer.Sanitize(request.Comment).Trim();

                if (string.IsNullOrWhiteSpace(cleanComment))
                    cleanComment = null;
            }

            var review = new Review(
                request.OrderItemId,
                request.Rating,
                cleanComment
            );

            if (request.ImageUrls != null)
            {
                foreach (var url in request.ImageUrls.Distinct())
                {
                    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                        throw new DomainException("URL ảnh không hợp lệ");

                    review.AddImage(url);
                }
            }

            await reviewRepo.AddAsync(review, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}