using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Entities;

public class Cart : AggregateRoot, IAuditableUser
{
    private readonly List<CartItem> _items = new();

    public Guid UserId { get; private set; }
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

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

    public void AddItem(Guid productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Số lượng phải lớn hơn 0.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            _items.Add(new CartItem(productId, quantity));
        else
            item.Increase(quantity);
    }

    public void UpdateQuantity(Guid productId, int quantity)
    {
        var item = GetItem(productId);
        item.SetQuantity(quantity);
    }

    public void RemoveItem(Guid productId)
    {
        var item = GetItem(productId);
        _items.Remove(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    private CartItem GetItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new ArgumentException("Không tìm thấy hàng trong giỏ.");

        return item;
    }
}
