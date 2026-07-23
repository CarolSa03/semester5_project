using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Docks.ValueObjects;
using PortManagement.Domain.Vessel.Entities;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class DockConfiguration : IEntityTypeConfiguration<Dock>
    {
        public void Configure(EntityTypeBuilder<Dock> b)
        {
            b.HasKey(d => d.Id);

            b.Property(d => d.Name)
             .HasConversion<string>(
                 n => n.Value,
                 v => new DockName(v))
             .HasMaxLength(100)
             .IsRequired();

            b.Property(d => d.Location)
             .HasConversion<string>(
                 loc => loc.Value,
                 v => new DockLocation(v))
             .HasMaxLength(200)
             .IsRequired();

            b.Property(d => d.Length)
             .HasConversion<int>(
                 l => l.Value,
                 v => new DockLength(v))
             .IsRequired();

            b.Property(d => d.Depth)
             .HasConversion<int>(
                 depth => depth.Value,
                 v => new DockDepth(v))
             .IsRequired();

            b.Property(d => d.MaxDraft)
             .HasConversion<int>(
                 md => md.Value,
                 v => new DockMaxDraft(v))
             .IsRequired();

            b.Property(d => d.MaxSTS)
             .HasConversion<int>(
                 s => s.Value,
                 v => new DockMaxSTS(v))
             .IsRequired();

            b.Property(d => d.IsActive).IsRequired();
            b.Property(d => d.CreatedAt).IsRequired();
            b.Property(d => d.UpdatedAt);

            // Configure Many-to-Many between Dock and VesselType with explicit join table
            b.HasMany(d => d.AllowedVesselTypes)
             .WithMany()
             .UsingEntity<Dictionary<string, object>>(
                 "DockVesselTypes",
                 j => j.HasOne<VesselType>()
                       .WithMany()
                       .HasForeignKey("VesselTypeId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j => j.HasOne<Dock>()
                       .WithMany()
                       .HasForeignKey("DockId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j =>
                 {
                     j.HasKey("DockId", "VesselTypeId");
                     j.ToTable("DockVesselTypes");
                 });
        }
    }
}
