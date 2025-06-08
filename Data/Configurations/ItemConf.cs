using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class ItemConf : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.ItemId);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Description)
            .HasMaxLength(200);

        builder.Property(i => i.Price)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.StockQuantity)
            .IsRequired();

        builder.Property(i => i.CategoryId)
            .IsRequired();

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId);

        builder.HasMany(i => i.OrderItems)
            .WithOne(oi => oi.Item)
            .HasForeignKey(oi => oi.ItemId);
    }
}