using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.PhysicalResources.Entities;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class PhysicalResourceConfiguration : IEntityTypeConfiguration<PhysicalResource>
    {
        public void Configure(EntityTypeBuilder<PhysicalResource> b)
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Id).ValueGeneratedNever();

            b.OwnsOne(p => p.Code, nb =>
                nb.Property(v => v.Value).HasColumnName("Code").HasMaxLength(20).IsRequired());

            b.OwnsOne(p => p.Description, nb =>
                nb.Property(v => v.Value).HasColumnName("Description").HasMaxLength(255).IsRequired());

            b.OwnsOne(p => p.Area, nb =>
                nb.Property(v => v.Value).HasColumnName("Area").HasMaxLength(64).IsRequired());

            b.OwnsOne(p => p.SetupTime, nb =>
                nb.Property(v => v.Minutes).HasColumnName("SetupTimeMinutes").IsRequired());

            b.OwnsOne(p => p.OperationalWindow, nb =>
                nb.Property(v => v.Value).HasColumnName("OperationalWindow").HasMaxLength(64).IsRequired());

            b.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            b.Property<List<Guid>>("_requiredQualificationIds")
                .HasColumnName("RequiredQualificationIds")
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Guid.Parse)
                          .ToList());

            b.HasDiscriminator<string>("ResourceType")
                .HasValue<Truck>("Truck")
                .HasValue<YardCrane>("YardCrane")
                .HasValue<STSCrane>("STSCrane");
        }
    }

    public class TruckConfiguration : IEntityTypeConfiguration<Truck>
    {
        public void Configure(EntityTypeBuilder<Truck> b)
        {
            b.OwnsOne(t => t.Capacity, nb =>
            {
                nb.Property(p => p.Value).HasColumnName("CapacityValue").HasMaxLength(20);
                nb.Property(p => p.Unit).HasColumnName("CapacityUnit").HasMaxLength(20);
            });

            b.OwnsOne(t => t.Speed, nb =>
            {
                nb.Property(p => p.Value).HasColumnName("SpeedValue").HasMaxLength(20);
                nb.Property(p => p.Unit).HasColumnName("SpeedUnit").HasMaxLength(20);
            });
        }
    }

    public class YardCraneConfiguration : IEntityTypeConfiguration<YardCrane>
    {
        public void Configure(EntityTypeBuilder<YardCrane> b)
        {
            b.OwnsOne(y => y.Capacity, nb =>
            {
                nb.Property(p => p.Value).HasColumnName("CapacityValue").HasMaxLength(20);
                nb.Property(p => p.Unit).HasColumnName("CapacityUnit").HasMaxLength(20);
            });
        }
    }

    public class STSCraneConfiguration : IEntityTypeConfiguration<STSCrane>
    {
        public void Configure(EntityTypeBuilder<STSCrane> b)
        {
            b.OwnsOne(c => c.Capacity, nb =>
            {
                nb.Property(p => p.Value).HasColumnName("CapacityValue").HasMaxLength(20);
                nb.Property(p => p.Unit).HasColumnName("CapacityUnit").HasMaxLength(20);
            });
        }
    }
}
