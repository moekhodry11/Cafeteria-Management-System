using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data;

public class WorkerConf : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        // 1. Table name
        builder.ToTable("Workers");

        // 2. Primary key
        builder.HasKey(w => w.WorkerId);

        // 3. Properties with constraints
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Password)
            .IsRequired()
            .HasMaxLength(255); // Increase for hashed passwords

        // Configure enum to be stored as string
        builder.Property(w => w.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(WorkerRole.Cashier);

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(w => w.CreatedDate)
            .IsRequired()
            .HasDefaultValue(DateTime.Now);

        // 4. Relationships
        builder.HasMany(w => w.Orders)
            .WithOne(o => o.Worker)
            .HasForeignKey(o => o.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. Indexes
        builder.HasIndex(w => w.Username)
            .IsUnique();
        builder.HasIndex(w => w.Role);
        builder.HasIndex(w => w.IsActive);
    }
}