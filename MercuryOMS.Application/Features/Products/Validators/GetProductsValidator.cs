using FluentValidation;

namespace MercuryOMS.Application.Features
{
    public class GetProductsValidator : AbstractValidator<GetProducts>
    {
        public GetProductsValidator()
        {
            RuleFor(x => x.Filter.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageIndex phải lớn hơn hoặc bằng 1");

            RuleFor(x => x.Filter.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize phải nằm trong khoảng từ 1 đến 100");

            RuleFor(x => x.Filter.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.MinPrice.HasValue)
                .WithMessage("MinPrice phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.Filter.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Filter.MaxPrice.HasValue)
                .WithMessage("MaxPrice phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.Filter)
                .Must(f => !f.MinPrice.HasValue || !f.MaxPrice.HasValue || f.MinPrice <= f.MaxPrice)
                .WithMessage("MinPrice không được lớn hơn MaxPrice");
        }
    }
}