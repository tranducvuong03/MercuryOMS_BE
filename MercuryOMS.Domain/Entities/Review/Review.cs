using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class Review : AuditableEntity
    {
        public Guid OrderItemId { get; private set; }

        public int Rating { get; private set; }
        public string? Comment { get; private set; }

        private readonly List<ReviewImage> _images = new();
        public IReadOnlyCollection<ReviewImage> Images => _images.AsReadOnly();

        private Review() { }

        public Review(Guid orderItemId, int rating, string? comment)
        {
            if (orderItemId == Guid.Empty)
                throw new DomainException("OrderItemId không hợp lệ");

            if (rating < 1 || rating > 5)
                throw new DomainException("Rating phải từ 1 đến 5");

            OrderItemId = orderItemId;
            Rating = rating;

            SetComment(comment);
        }

        public void SetComment(string? comment)
        {
            if (comment != null)
            {
                comment = comment.Trim();

                if (comment.Length > 1000)
                    throw new DomainException("Comment quá dài (max 1000 ký tự)");

                if (string.IsNullOrWhiteSpace(comment))
                    comment = null;
            }

            Comment = comment;
        }

        public void AddImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new DomainException("URL ảnh không hợp lệ");

            if (_images.Count >= 5)
                throw new DomainException("Tối đa 5 ảnh");

            if (_images.Any(x => x.Url == url))
                return;

            _images.Add(new ReviewImage(Id, url));
        }

        public void RemoveImage(Guid imageId)
        {
            var img = _images.FirstOrDefault(x => x.Id == imageId);
            if (img != null)
                _images.Remove(img);
        }
    }
}