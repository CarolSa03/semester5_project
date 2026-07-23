using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.ShippingAgent.Entities;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class ShippingAgentOrganizationConfiguration : IEntityTypeConfiguration<ShippingAgentOrganization>
    {
        public void Configure(EntityTypeBuilder<ShippingAgentOrganization> b)
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).HasMaxLength(100);
            b.Property(s => s.LegalName).IsRequired().HasMaxLength(200);
            b.Property(s => s.AlternativeName).HasMaxLength(200);
            b.Property(s => s.TaxNumber).HasMaxLength(50);

            b.HasMany(s => s.Representatives)
             .WithOne(r => r.ShippingAgentOrganization)
             .HasForeignKey(r => r.ShippingAgentOrganizationId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ShippingAgentRepresentativeConfiguration : IEntityTypeConfiguration<ShippingAgentRepresentative>
    {
        public void Configure(EntityTypeBuilder<ShippingAgentRepresentative> b)
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).HasMaxLength(100);
            b.Property(s => s.Name).IsRequired().HasMaxLength(200);
            b.Property(s => s.Email).HasMaxLength(100);
            b.Property(s => s.Phone).HasMaxLength(50);
        }
    }
}
