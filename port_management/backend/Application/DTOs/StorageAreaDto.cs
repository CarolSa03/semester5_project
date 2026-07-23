using System.ComponentModel.DataAnnotations;

namespace PortManagement.Application.DTOs
{
    // Data Transfer Object for storage area information
    public class StorageAreaDto
    {
        public Guid? Id { get; set; } // Database primary key

        [Required(ErrorMessage = "Storage area ID is required")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "ID must be between 5 and 10 characters")]
        public string BusinessId { get; set; } = string.Empty; // Business identifier (SA-XXX)

        [Required(ErrorMessage = "Storage area type is required")]
        public string Type { get; set; } = string.Empty; // Type of storage (Yard, Warehouse)

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Location must be between 3 and 100 characters")]
        public string Location { get; set; } = string.Empty; // Physical location in the port

        [Range(0, 10000, ErrorMessage = "Maximum capacity must be between 0 and 10,000 TEU")]
        public int MaxCapacityTEU { get; set; } // Maximum capacity in TEU

        [Range(0, 10000, ErrorMessage = "Current capacity must be between 0 and 10,000 TEU")]
        public int CurrentCapacityTEU { get; set; } // Current occupancy in TEU

        public List<Guid>? ServedDocks { get; set; }

        public Dictionary<int, double>? DockDistances { get; set; } // Distance from this storage area to each dock (by dock ID)

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; } // Additional notes or comments
    }
}
