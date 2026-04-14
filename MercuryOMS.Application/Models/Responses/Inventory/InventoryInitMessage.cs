namespace MercuryOMS.Application.Models.Responses
{
    public class InventoryInitMessage
    {
        public Guid VariantId { get; set; }
        public int InitialQuantity { get; set; }
    }
}
