using System;
using System.Collections.Generic;

namespace Data;

public class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Add payment method
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    // Add payment status
    public bool IsPaid { get; set; } = false;

    public DateTime? PaidDate { get; set; }

    public string? Notes { get; set; }

    // Foreign keys
    public int? TableId { get; set; }

    public int WorkerId { get; set; }

    // Navigation properties
    public virtual Table? Table { get; set; }

    public virtual Worker Worker { get; set; } = null!;
        
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}