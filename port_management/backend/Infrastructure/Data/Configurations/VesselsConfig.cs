using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Domain.Docks.Entities;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class VesselTypeConfiguration : IEntityTypeConfiguration<VesselType>
    {
        public void Configure(EntityTypeBuilder<VesselType> b)
        {
            b.HasKey(vt => vt.Id);

            b.Property(vt => vt.Name)
             .HasConversion<string>(
                 n => n.Value,
                 v => new VTName(v))
             .HasMaxLength(100)
             .IsRequired();

            b.Property(vt => vt.Description)
             .HasConversion<string>(
                 d => d.Value,
                 v => new VTDescription(v))
             .HasMaxLength(1000);

            b.Property(vt => vt.CapacityTEU)
             .HasConversion<int>(
                 c => c.Value,
                 v => new VTCapacityTEU(v))
             .IsRequired();

            b.Property(vt => vt.MaxRows)
             .HasConversion<int>(
                 r => r.Value,
                 v => new VTMaxRows(v))
             .IsRequired();

            b.Property(vt => vt.MaxBays)
             .HasConversion<int>(
                 bays => bays.Value,
                 v => new VTMaxBays(v))
             .IsRequired();

            b.Property(vt => vt.MaxTiers)
             .HasConversion<int>(
                 t => t.Value,
                 v => new VTMaxTiers(v))
             .IsRequired();

            b.Property(vt => vt.IsActive).IsRequired();
            b.Property(vt => vt.CreatedAt).IsRequired();
            b.Property(vt => vt.UpdatedAt);
        }
    }

    public class VesselRecordConfiguration : IEntityTypeConfiguration<VesselRecord>
    {
        public void Configure(EntityTypeBuilder<VesselRecord> b)
        {
            b.HasKey(v => v.Id);

            b.Property(v => v.Imo)
             .HasConversion(imo => imo.Value, v => new Imo(v))
             .HasMaxLength(10)
             .IsRequired();

            b.HasIndex(v => v.Imo)
             .IsUnique();

            b.Property(v => v.Name)
             .HasConversion(name => name.Value, v => new VRName(v))
             .IsRequired()
             .HasMaxLength(100);

            b.Property(v => v.Owner)
             .HasConversion(owner => owner.Value, v => new VROwner(v))
             .IsRequired()
             .HasMaxLength(120);

            b.Property(v => v.Status)
             .HasConversion<string>()
             .IsRequired();

            b.Property(v => v.CreatedAt)
             .IsRequired();

            b.Property(v => v.UpdatedAt);

            b.HasOne(v => v.VesselType)
             .WithMany()
             .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class VesselVisitNotificationConfiguration : IEntityTypeConfiguration<VesselVisitNotification>
    {
        public void Configure(EntityTypeBuilder<VesselVisitNotification> b)
        {
            // Use GUID Id as the primary key for EF and persist the BusinessId value as string.
            b.HasKey(v => v.Id);

            b.Property("_businessId")
                .HasColumnName("BusinessId")
                .HasMaxLength(100)
                .IsRequired();

            b.Ignore(v => v.BusinessId);

            b.HasIndex("_businessId")
                .IsUnique()
                .HasDatabaseName("IX_VesselVisitNotification_BusinessId");
            
            b.HasOne(v => v.Vessel).WithMany().OnDelete(DeleteBehavior.Restrict);
            b.HasOne(v => v.ShippingAgentRepresentative).WithMany().OnDelete(DeleteBehavior.Restrict);

            // Assigned dock: keep reference and allow null if unassigned
            b.HasOne(v => v.AssignedDock)
             .WithMany()
             .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(v => v.Crew).WithMany().OnDelete(DeleteBehavior.Cascade);

            b.HasMany(v => v.CargoManifests).WithOne().OnDelete(DeleteBehavior.Cascade);

            b.Property(v => v.Status).HasConversion<string>();

            b.Property(v => v.ApprovalNotes)
             .HasConversion<string?>(
                 n => n,
                 v => v)
             .HasMaxLength(1000);

            b.Property(v => v.RejectionReason)
             .HasConversion<string?>(
                 r => r,
                 v => v)
             .HasMaxLength(500);

            b.Property(v => v.CreatedAt).IsRequired();
            b.Property(v => v.UpdatedAt);
        }
    }

    public class CargoManifestConfiguration : IEntityTypeConfiguration<CargoManifest>
    {
        public void Configure(EntityTypeBuilder<CargoManifest> b)
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Type).HasConversion<string>();

            b.HasMany(c => c.Containers)
             .WithOne(ci => ci.CargoManifest)
             .HasForeignKey(ci => ci.CargoManifestId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ContainerInfoConfiguration : IEntityTypeConfiguration<ContainerInfo>
    {
        public void Configure(EntityTypeBuilder<ContainerInfo> b)
        {
            b.HasKey(c => c.Id);

            // ContainerId is a value object
            b.Property(c => c.ContainerId)
             .HasConversion<string?>(
                 id => id != null ? id.Value : null,
                 v => v != null ? new CMContainerId(v) : null)
             .HasMaxLength(50);

            // CargoType is a value object
            b.Property(c => c.CargoType)
             .HasConversion<string?>(
                 ct => ct != null ? ct.Value : null,
                 v => v != null ? new CMCargoType(v) : null)
             .HasMaxLength(200);

            b.Property(c => c.Description).HasMaxLength(500);

            b.Property(c => c.Bay).IsRequired();
            b.Property(c => c.Row).IsRequired();
            b.Property(c => c.Tier).IsRequired();
        }
    }

    public class CrewInfoConfiguration : IEntityTypeConfiguration<CrewInfo>
    {
        public void Configure(EntityTypeBuilder<CrewInfo> b)
        {
            b.HasKey(c => c.Id);

            // CaptainName nullable value object
            b.Property(c => c.CaptainName)
             .HasConversion<string?>(
                 n => n != null ? n.Value : null,
                 v => v != null ? new CICaptainName(v) : null)
             .HasMaxLength(200);

            b.Property(c => c.CrewCount)
             .HasConversion<int>(
                 cc => cc.Value,
                 v => new CICrewCount(v))
             .IsRequired();

            b.HasMany(c => c.SafetyOfficers)
             .WithOne()
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class CrewMemberConfiguration : IEntityTypeConfiguration<CrewMember>
    {
        public void Configure(EntityTypeBuilder<CrewMember> b)
        {
            b.HasKey(c => c.Id);

            b.Property(c => c.Name)
             .HasConversion<string?>(
                 n => n != null ? n.Value : null,
                 v => v != null ? new CICrewMemberName(v) : null)
             .HasMaxLength(200);

            b.Property(c => c.CitizenId)
             .HasConversion<string?>(
                 id => id != null ? id.Value : null,
                 v => v != null ? new CICitizenId(v) : null)
             .HasMaxLength(100);

            b.Property(c => c.Nationality)
             .HasConversion<string?>(
                 n => n != null ? n.Value : null,
                 v => v != null ? new CINationality(v) : null)
             .HasMaxLength(100);
        }
    }
}
