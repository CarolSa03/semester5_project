using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.Qualification.Entities;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
    {
        public void Configure(EntityTypeBuilder<Qualification> b)
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Id).ValueGeneratedNever();

            // --- Code ---
            b.OwnsOne(q => q.Code, nb =>
            {
                nb.Property(v => v.Value)
                    .HasColumnName("Code")
                    .HasMaxLength(15)
                    .IsRequired();

                nb.HasIndex(v => v.Value).IsUnique();
            });

            // --- Description ---
            b.OwnsOne(q => q.Description, d =>
            {
                d.Property(p => p.Value)
                    .HasColumnName("Description")
                    .HasMaxLength(150)
                    .IsRequired();
            });
        }
    }
}