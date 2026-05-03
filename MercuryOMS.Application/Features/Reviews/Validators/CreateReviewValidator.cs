using FluentValidation;
using MercuryOMS.Application.Features;

public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.OrderItemId)
            .NotEmpty()
            .WithMessage("OrderItemId không hợp lệ");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating phải từ 1 đến 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment tối đa 1000 ký tự");

        RuleFor(x => x.ImageUrls)
            .Must(x => x == null || x.Count <= 5)
            .WithMessage("Tối đa 5 ảnh")
            .Must(AllValidUrls)
            .WithMessage("URL ảnh không hợp lệ");
    }

    private bool AllValidUrls(List<string>? urls)
    {
        if (urls == null) return true;

        return urls.All(url =>
            !string.IsNullOrWhiteSpace(url) &&
            Uri.TryCreate(url, UriKind.Absolute, out _));
    }
}