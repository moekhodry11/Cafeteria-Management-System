using Microsoft.EntityFrameworkCore;

namespace Data;

public class MyContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        
        optionsBuilder.UseSqlServer(
            "Server=.;Database=Cafeteria;Trusted_Connection=True;TrustServerCertificate=True;"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}