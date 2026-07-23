using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.Auth;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.EventType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Details)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.PerformedBy)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.AffectedUser)
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(a => a.IpAddress)
                .IsRequired(false)
                .HasMaxLength(45);

            builder.Property(a => a.Metadata)
                .IsRequired(false)
                .HasMaxLength(4000);

            builder.HasIndex(a => a.EventType);
            builder.HasIndex(a => a.PerformedBy);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.AffectedUser);
        }
    }
}
