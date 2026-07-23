using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Domain.Vessel.Entities
{

    public class CargoManifest
    {
        public Guid Id { get; set; }
        public Guid VesselVisitNotificationId { get; set; }
        public ManifestType Type { get; set; } // Type of cargo operation
        public List<ContainerInfo>? Containers { get; set; } // List of containers in this manifest

        // Check if the manifest contains any containers
        public bool HasContainers()
        {
            return Containers != null && Containers.Any();
        }

        // Check if the manifest contains any dangerous cargo types
        public bool HasDangerousCargo()
        {
            var dangerousTypes = new[] { "HAZMAT", "CHEMICAL", "RADIOACTIVE", "EXPLOSIVE", "DANGEROUS" };
            return Containers?.Any(container =>
                container.CargoType != null && dangerousTypes.Contains(container.CargoType.Value.ToUpper())) == true;
        }

        public void Validate()
        {
            if (Containers == null) return;
            foreach (var container in Containers)
            {
                if (container.ContainerId == null || string.IsNullOrEmpty(container.ContainerId.Value))
                    throw new InvalidOperationException("Container ID is required");
                if (container.CargoType == null || string.IsNullOrEmpty(container.CargoType.Value))
                    throw new InvalidOperationException("Cargo type is required");
                if (container.Bay < 0 || container.Row < 0 || container.Tier < 0)
                    throw new InvalidOperationException("Bay, Row, and Tier must be positive values");
            }
        }
    }

    public class ContainerInfo
    {
        public Guid Id { get; set; }  // Primary key
        public Guid CargoManifestId { get; set; }  // Foreign key
        public CMContainerId? ContainerId { get; set; }
        public string? Description { get; set; }
        public CMCargoType? CargoType { get; set; }
        public int Bay { get; set; }
        public int Row { get; set; }
        public int Tier { get; set; }
        public CargoManifest? CargoManifest { get; set; }
    }
}
