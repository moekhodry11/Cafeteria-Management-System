using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace Data;

public class OrderConf : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // 1. Table name
        builder.ToTable("Orders");

        // 2. Primary key
        builder.HasKey(o => o.OrderId);

        // 3. Properties with constraints
        builder.Property(o => o.OrderDate)
            .IsRequired()
            .HasDefaultValue(DateTime.Now);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasPrecision(10, 2);

        // Configure OrderStatus enum
        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(OrderStatus.Pending);

        // Configure PaymentMethod enum
        builder.Property(o => o.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PaymentMethod.Cash);

        builder.Property(o => o.IsPaid)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.PaidDate);

        builder.Property(o => o.Notes)
            .HasMaxLength(200);

        builder.Property(o => o.WorkerId)
            .IsRequired();

        // 4. Relationships
        builder.HasOne(o => o.Table)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Worker)
            .WithMany(w => w.Orders)
            .HasForeignKey(o => o.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. Indexes
        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.PaymentMethod);
        builder.HasIndex(o => o.IsPaid);
        builder.HasIndex(o => o.WorkerId);
    }
}