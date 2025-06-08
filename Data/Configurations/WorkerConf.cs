using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class WorkerConf : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.HasKey(w => w.WorkerId);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Password)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Role)
            .HasMaxLength(50);

        builder.HasMany(w => w.Orders)
            .WithOne(o => o.Worker)
            .HasForeignKey(o => o.WorkerId);
    }
}