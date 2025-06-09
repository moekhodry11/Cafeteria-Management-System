using System;
using System.Collections.Generic;

namespace Data;

public class Worker
{
    public int WorkerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty; // In production, use hashed passwords

    public WorkerRole Role { get; set; } = WorkerRole.Cashier;
        
    public bool IsActive { get; set; } = true;
        
    public DateTime CreatedDate { get; set; } = DateTime.Now;
        
    // Navigation property
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
}