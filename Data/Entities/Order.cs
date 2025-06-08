namespace Data;

public class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

    public string Notes { get; set; }

    // Foreign keys
    public int? TableId { get; set; }

    public int WorkerId { get; set; }

    // Navigation properties
    public virtual Table Table { get; set; }

    public virtual Worker Worker { get; set; }
        
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}