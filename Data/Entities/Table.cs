namespace Data;

public class Table
{
    public int TableId { get; set; }

    public string TableNumber { get; set; }
        
    public int Capacity { get; set; }
        
    public bool IsOccupied { get; set; } = false;
        
    // Navigation property
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
}