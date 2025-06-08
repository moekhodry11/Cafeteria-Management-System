namespace Data;

public class Category
{
    public int CategoryId { get; set; }
        
    public string Name { get; set; }

    public string Description { get; set; }
        
    // Navigation property
    public virtual ICollection<Item> Items { get; set; } = new HashSet<Item>();
}