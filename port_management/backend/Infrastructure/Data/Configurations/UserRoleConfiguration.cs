using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.Auth;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.AssignedBy)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(r => new { r.UserId, r.RoleName })
                .IsUnique();
        }
    }
}
