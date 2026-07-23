using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Auth;
using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.Qualification.Entities;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Domain.Staff.Entities;
using PortManagement.Domain.Storage.Entities;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Privacy;

namespace PortManagement.Infrastructure.Data
{
    public class PortManagementDbContext : DbContext
    {
        public PortManagementDbContext(DbContextOptions<PortManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Dock> Docks => Set<Dock>();
        public DbSet<VesselType> VesselTypes => Set<VesselType>();
        public DbSet<VesselRecord> VesselRecords => Set<VesselRecord>();
        public DbSet<VesselVisitNotification> VesselVisitNotifications => Set<VesselVisitNotification>();
        public DbSet<CargoManifest> CargoManifests => Set<CargoManifest>();
        public DbSet<ContainerInfo> ContainerInfos => Set<ContainerInfo>();
        public DbSet<CrewInfo> CrewInfos => Set<CrewInfo>();
        public DbSet<CrewMember> CrewMembers => Set<CrewMember>();
        public DbSet<ShippingAgentOrganization> ShippingAgentOrganizations => Set<ShippingAgentOrganization>();
        public DbSet<ShippingAgentRepresentative> ShippingAgentRepresentatives => Set<ShippingAgentRepresentative>();
        public DbSet<StorageArea> StorageAreas => Set<StorageArea>();
        public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
        public DbSet<Qualification> Qualifications => Set<Qualification>();
        public DbSet<PhysicalResource> PhysicalResources => Set<PhysicalResource>();
        public DbSet<Truck> Trucks => Set<Truck>();
        public DbSet<YardCrane> YardCranes => Set<YardCrane>();
        public DbSet<STSCrane> STSCranes => Set<STSCrane>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        public DbSet<UserPolicyView> UserPolicyViews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortManagementDbContext).Assembly);
        }
    }
}
