using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class CategoryConf : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // 1. Table name
        builder.ToTable("Categories");
        
        // 2. Primary key
        builder.HasKey(c => c.CategoryId);

        // 3. Properties with constraints
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(200);

        // 4. Relationships
        builder.HasMany(c => c.Items)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();
    }
}