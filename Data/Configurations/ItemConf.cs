using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data;

public class ItemConf : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        // 1. Table name
        builder.ToTable("Items");

        // 2. Primary key
        builder.HasKey(i => i.ItemId);

        // 3. Properties with constraints
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Description)
            .HasMaxLength(200);

        builder.Property(i => i.Price)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasPrecision(10, 2);

        builder.Property(i => i.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.CategoryId)
            .IsRequired();

        // Configure enum to be stored as string
        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(ItemStatus.Available);

        // 4. Relationships
        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.OrderItems)
            .WithOne(oi => oi.Item)
            .HasForeignKey(oi => oi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. Indexes
        builder.HasIndex(i => i.Name);
        builder.HasIndex(i => i.CategoryId);
        builder.HasIndex(i => i.Status);
    }
}