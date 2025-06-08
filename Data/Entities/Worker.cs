namespace Data;

public class Worker
{
    public int WorkerId { get; set; }

    public string Name { get; set; }

    public string Username { get; set; }

    public string Password { get; set; } // In production, use hashed passwords

    public string Role { get; set; } = "Cashier";
        
    public bool IsActive { get; set; } = true;
        
    public DateTime CreatedDate { get; set; } = DateTime.Now;
        
    // Navigation property
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
}