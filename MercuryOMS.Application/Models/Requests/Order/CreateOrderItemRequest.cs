namespace MercuryOMS.Application.Models.Requests
{
    public record CreateOrderItemRequest(
        Guid ProductId,
        int Quantity
    );
}
