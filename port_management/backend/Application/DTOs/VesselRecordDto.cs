using System.ComponentModel.DataAnnotations;

namespace PortManagement.Application.DTOs
{
    // Data Transfer Object for vessel record information
    public class VesselRecordDto
    {
        public Guid? Id { get; set; } // Database primary key

        [Required(ErrorMessage = "IMO number is required")]
        public string ImoValue { get; set; } = string.Empty; // IMO number (unique vessel identifier)

        [Required(ErrorMessage = "Vessel name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Vessel name must be between 3 and 100 characters")]
        public string Name { get; set; } = string.Empty; // Name of the vessel

        [Required(ErrorMessage = "Vessel type is required")]
        public Guid VesselTypeId { get; set; } // Type of the vessel

        [Required(ErrorMessage = "Owner/operator is required")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "Owner/operator must be between 2 and 120 characters")]
        public string Owner { get; set; } = string.Empty; // Owner or operator of the vessel

        public bool IsActive { get; set; } // Indicates if the vessel is active
        public DateTime CreatedAt { get; set; } // When the vessel record was created
        public DateTime? UpdatedAt { get; set; } // When the vessel record was last updated
    }
}
