using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class OrderItemConf : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // 1. Table name
        builder.ToTable("OrderItems");

        // 2. Primary key
        builder.HasKey(oi => oi.OrderItemId);

        // 3. Properties with constraints
        builder.Property(oi => oi.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasPrecision(10, 2);

        builder.Property(oi => oi.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasPrecision(10, 2);

        builder.Property(oi => oi.OrderId)
            .IsRequired();

        builder.Property(oi => oi.ItemId)
            .IsRequired();

        // 4. Relationships
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.Item)
            .WithMany(i => i.OrderItems)
            .HasForeignKey(oi => oi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. Indexes
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ItemId);
    }
}