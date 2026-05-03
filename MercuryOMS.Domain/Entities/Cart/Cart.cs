using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Exceptions;

public class Cart : AggregateRoot, IAuditableUser
{
    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    public Guid UserId { get; private set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? CreatedBy { get; set; }

    private Cart() { }

    public Cart(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
    }

    public void AddItem(Guid productId, Guid variantId, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Số lượng phải lớn hơn 0.");

        var item = Items.FirstOrDefault(i =>
            i.ProductId == productId &&
            i.VariantId == variantId);

        if (item != null)
        {
            item.Increase(quantity);
            return;
        }

        Items.Add(new CartItem(this.Id, productId, variantId, quantity));
    }

    public void RemoveItem(Guid productId, Guid variantId)
    {
        var item = Items.FirstOrDefault(i =>
            i.ProductId == productId &&
            i.VariantId == variantId);

        if (item == null)
            throw new DomainException("Không tìm thấy sản phẩm trong giỏ.");

        Items.Remove(item);
    }

    public void Clear()
    {
        Items.Clear();
    }
}