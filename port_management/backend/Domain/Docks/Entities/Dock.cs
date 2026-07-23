using System.ComponentModel.DataAnnotations;
using PortManagement.Domain.Docks.ValueObjects;
using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.Vessel.Entities;

namespace PortManagement.Domain.Docks.Entities;

// Represents a dock in the port
public class Dock
{
    [Required]
    public Guid Id { get; set; }                              // Unique identifier for the dock

    [Required]
    public DockName Name { get; set; }                         // Name or number of the dock

    [Required]
    public DockLocation Location { get; set; }                     // Location within the port area
    [Required]
    public DockLength Length { get; set; }                          // Length in meters
    [Required]
    public DockDepth Depth { get; set; }                           // Water depth in meters
    [Required]
    public DockMaxDraft MaxDraft { get; set; }                        // Maximum vessel draft allowed
    [Required]
    public List<VesselType> AllowedVesselTypes { get; set; } = [];  // Types of vessels that can use this dock
    [Required]
    public DockMaxSTS MaxSTS { get; set; }                          // Maximum number of simultaneous ship-to-ship operations
    [Required]
    public List<STSCrane> STSCranes { get; set; } = [];           // IDs of cranes available for STS operations
    [Required]
    public bool IsActive { get; set; } = true;               // Indicates if the dock is active (soft delete)
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Date and time when the dock was created
    [Required]
    public DateTime? UpdatedAt { get; set; }                 // Date and time when the dock was last updated

}
