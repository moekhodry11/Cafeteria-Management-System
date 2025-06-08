namespace Data;

public class Item
{
    public int ItemId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Foreign key
    public int CategoryId { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; }
        
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}