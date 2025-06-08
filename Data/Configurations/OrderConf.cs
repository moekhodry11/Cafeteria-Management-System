using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class OrderConf : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Notes)
            .HasMaxLength(200);

        builder.Property(o => o.WorkerId)
            .IsRequired();

        builder.HasOne(o => o.Table)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.TableId);

        builder.HasOne(o => o.Worker)
            .WithMany(w => w.Orders)
            .HasForeignKey(o => o.WorkerId);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);
    }
}