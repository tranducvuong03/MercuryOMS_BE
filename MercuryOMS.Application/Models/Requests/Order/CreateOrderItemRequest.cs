namespace MercuryOMS.Application.Models.Requests
{
    public record CreateOrderItemRequest(
        Guid ProductVariantId,
        int Quantity
    );
}
