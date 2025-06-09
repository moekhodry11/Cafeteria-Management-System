using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data;

public class Item
{
    public int ItemId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    // Replaced boolean with enum
    public ItemStatus Status { get; set; } = ItemStatus.Available;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? UpdatedDate { get; set; }

    // Foreign Keys
    [Required]
    public int CategoryId { get; set; }

    // Navigation Properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}