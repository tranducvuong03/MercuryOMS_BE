using FluentValidation;
using MercuryOMS.Domain.Constants;

namespace MercuryOMS.Application.Features
{
    public class AddToCartValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(Message.CartProductIdRequired);

            RuleFor(x => x.VariantId)
                .NotEmpty()
                .WithMessage("VariantId là bắt buộc");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage(Message.CartQuantityGreaterThanZero)
                .LessThanOrEqualTo(100)
                .WithMessage(Message.CartQuantityMaxExceeded);
        }
    }
}