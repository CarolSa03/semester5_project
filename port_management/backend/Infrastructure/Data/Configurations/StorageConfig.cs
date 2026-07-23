using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.Storage.Entities;
using PortManagement.Domain.Storage.ValueObjects;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class StorageAreaConfiguration : IEntityTypeConfiguration<StorageArea>
    {
        public void Configure(EntityTypeBuilder<StorageArea> b)
        {
            b.HasKey(s => s.Id);

            b.Property<SAId>("BusinessId")
             .HasConversion(
                 id => id.Value,
                 v => new SAId(v))
             .HasMaxLength(10)
             .IsRequired();

            b.HasIndex("BusinessId")
             .IsUnique();

            b.Property(s => s.Type)
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired();

            b.Property(s => s.Location)
             .HasConversion<string>(
                 loc => loc.Value,
                 v => new SALocation(v))
             .HasMaxLength(100)
             .IsRequired();

            b.Property(s => s.MaxCapacity)
             .HasConversion(
                 cap => cap.Value,
                 v => new SACapacity(v))
             .IsRequired();

            b.Property(s => s.CurrentCapacity)
             .HasConversion(
                 cap => cap.Value,
                 v => new SACapacity(v))
             .IsRequired();

            b.Property(s => s.Notes)
             .HasConversion<string?>(
                 notes => notes != null ? notes.Value : null,
                 v => v != null ? new SANotes(v) : null)
             .HasMaxLength(500);

            b.Ignore(s => s.DockDistances);

            b.HasMany(s => s.ServedDocks)
             .WithMany()
             .UsingEntity(j => j.ToTable("StorageAreaDocks"));
        }
    }
}
