using FluentValidation;
using MercuryOMS.Domain.Constants;

namespace MercuryOMS.Application.Features
{
    public class GetProductsValidator : AbstractValidator<GetProducts>
    {
        public GetProductsValidator()
        {
            RuleFor(x => x.Filter.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage(Message.PageIndexInvalid);

            RuleFor(x => x.Filter.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage(Message.PageSizeInvalid);

            RuleFor(x => x.Filter.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.MinPrice.HasValue)
                .WithMessage(Message.MinPriceInvalid);

            RuleFor(x => x.Filter.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.MaxPrice.HasValue)
                .WithMessage(Message.MaxPriceInvalid);

            RuleFor(x => x.Filter)
                .Must(f => !f.MinPrice.HasValue || !f.MaxPrice.HasValue || f.MinPrice <= f.MaxPrice)
                .WithMessage(Message.MinPriceGreaterThanMaxPrice);
        }
    }
}