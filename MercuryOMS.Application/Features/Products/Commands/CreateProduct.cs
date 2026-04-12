using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.UOW;
using MercuryOMS.Application.Models.Requests;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Application.Features
{
    public record CreateProductCommand(
        string Name,
        decimal BasePrice,
        string? Description,
        List<Guid>? CategoryIds,
        List<string>? ImageUrls,
        List<CreateVariantRequest>? Variants
    ) : IRequest<Result<Guid>>;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
        {
            var product = CreateProduct(request);

            var categoryResult = await HandleCategoriesAsync(product, request.CategoryIds, ct);
            if (!categoryResult.IsSuccess)
                return categoryResult;

            HandleImages(product, request.ImageUrls);
            HandleVariants(product, request.Variants);

            await SaveProductAsync(product, ct);

            return Result<Guid>.Success(product.Id, Message.ProductCreated);
        }

        private static Product CreateProduct(CreateProductCommand request)
        {
            var product = new Product(request.Name, request.BasePrice);
            product.SetDescription(request.Description);
            return product;
        }

        private async Task<Result<Guid>> HandleCategoriesAsync(
            Product product,
            List<Guid>? categoryIds,
            CancellationToken ct)
        {
            if (categoryIds == null || !categoryIds.Any())
                return Result<Guid>.Success(Guid.Empty);

            var categoryRepo = _unitOfWork.GetRepository<Category>();

            var categories = await categoryRepo.Query
                .Where(x => categoryIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(ct);

            if (categories.Count != categoryIds.Count)
                return Result<Guid>.Failure(Message.CategoryNotFound);

            foreach (var categoryId in categories)
            {
                product.AddCategory(categoryId);
            }

            return Result<Guid>.Success(Guid.Empty);
        }

        private static void HandleImages(Product product, List<string>? imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any()) return;

            for (int i = 0; i < imageUrls.Count; i++)
            {
                product.AddImage(imageUrls[i], isPrimary: i == 0);
            }
        }

        private static void HandleVariants(Product product, List<CreateVariantRequest>? variants)
        {
            if (variants == null) return;

            foreach (var v in variants)
            {
                product.AddVariant(v.Sku, v.Price, v.Color, v.Size);
            }
        }

        private async Task SaveProductAsync(Product product, CancellationToken ct)
        {
            var productRepo = _unitOfWork.GetRepository<Product>();
            await productRepo.AddAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
