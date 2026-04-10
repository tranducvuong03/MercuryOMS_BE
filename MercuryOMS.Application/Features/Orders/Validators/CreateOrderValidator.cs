using FluentValidation;
using MercuryOMS.Application.Models.Requests;
using MercuryOMS.Domain.Constants;

namespace MercuryOMS.Application.Features
{
    public class CreateOrderCommandValidator
        : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Items)
                .NotNull().WithMessage(Message.OrderEmpty)
                .NotEmpty().WithMessage(Message.OrderEmpty);

            RuleForEach(x => x.Items)
                .SetValidator(new CreateOrderItemRequestValidator());
        }
    }

    public class CreateOrderItemRequestValidator
        : AbstractValidator<CreateOrderItemRequest>
    {
        public CreateOrderItemRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage(Message.CartProductIdRequired);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage(Message.OrderQuantityInvalid);
        }
    }
}