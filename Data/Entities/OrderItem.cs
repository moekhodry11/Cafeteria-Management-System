namespace Data;

public class OrderItem
{
    public int OrderItemId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    // Foreign keys
    public int OrderId { get; set; }

    public int ItemId { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }

    public virtual Item Item { get; set; }
}