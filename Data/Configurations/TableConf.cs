using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class TableConf : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        // 1. Table name
        builder.ToTable("Tables");

        // 2. Primary key
        builder.HasKey(t => t.TableId);

        // 3. Properties with constraints
        builder.Property(t => t.TableNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Capacity)
            .IsRequired()
            .HasDefaultValue(4);

        builder.Property(t => t.IsOccupied)
            .IsRequired()
            .HasDefaultValue(false);

        // 4. Relationships
        builder.HasMany(t => t.Orders)
            .WithOne(o => o.Table)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. Indexes
        builder.HasIndex(t => t.TableNumber)
            .IsUnique();
        builder.HasIndex(t => t.IsOccupied);
    }
}