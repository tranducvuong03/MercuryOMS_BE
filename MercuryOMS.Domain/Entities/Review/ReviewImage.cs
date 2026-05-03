using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class ReviewImage : BaseEntity
    {
        public Guid ReviewId { get; private set; }
        public string Url { get; private set; } = null!;

        private ReviewImage() { }

        public ReviewImage(Guid reviewId, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new DomainException("URL ảnh không hợp lệ");

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new DomainException("URL ảnh không hợp lệ");

            ReviewId = reviewId;
            Url = url;
        }
    }
}
