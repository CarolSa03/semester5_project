using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortManagement.Domain.Staff.Entities;

namespace PortManagement.Infrastructure.Data.Configurations
{
    public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
    {
        public void Configure(EntityTypeBuilder<StaffMember> b)
        {
            // Chave primária
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).ValueGeneratedNever();

            // --- Value Objects ---

            b.OwnsOne(s => s.StaffMemberId, nb =>
                nb.Property(v => v.Value)
                  .HasColumnName("StaffMemberId")
                  .HasMaxLength(50)
                  .IsRequired());

            b.OwnsOne(s => s.ShortName, nb =>
                nb.Property(v => v.Value)
                  .HasColumnName("ShortName")
                  .HasMaxLength(100)
                  .IsRequired());

            b.OwnsOne(s => s.Email, nb =>
                nb.Property(v => v.Value)
                  .HasColumnName("Email")
                  .HasMaxLength(150)
                  .IsRequired());

            b.OwnsOne(s => s.PhoneNumber, nb =>
                nb.Property(v => v.Value)
                  .HasColumnName("PhoneNumber")
                  .HasMaxLength(30)
                  .IsRequired());

            b.OwnsOne(s => s.OperationalWindow, nb =>
                nb.Property(v => v.Value)
                  .HasColumnName("OperationalWindow")
                  .HasMaxLength(64)
                  .IsRequired());

            // --- Disponibilidade ---
            b.Property(s => s.IsAvailable)
                .IsRequired();

            // --- Lista de Qualificações (armazenada como CSV de GUIDs) ---
            b.Property<List<Guid>>("_qualificationIds")
                .HasColumnName("QualificationIds")
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Guid.Parse)
                          .ToList());

            // --- Nome da tabela ---
            b.ToTable("StaffMembers");
        }
    }
}
