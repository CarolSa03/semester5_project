using System.ComponentModel.DataAnnotations;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Docks.ValueObjects;
using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.Vessel.Entities;

namespace PortManagement.Application.DTOs;

// Data Transfer Object for Dock entity
public class DockDto
{
    public Guid Id { get; set; } // Dock identifier (optional for creation)

    [Required(ErrorMessage = "Dock name is required")]
    [StringLength(50, ErrorMessage = "Dock name cannot exceed 50 characters")]
    public string Name { get; set; } // Name of the dock

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    public string Location { get; set; } // Location of the dock

    [Required(ErrorMessage = "Length is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Length must be greater than 0")]
    public int Length { get; set; } // Length in meters

    [Required(ErrorMessage = "Depth is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Depth must be greater than 0")]
    public int Depth { get; set; } // Water depth in meters

    [Required(ErrorMessage = "Max draft is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Max draft must be greater than 0")]
    public int MaxDraft { get; set; } // Maximum vessel draft allowed

    [Required(ErrorMessage = "Max STS cranes is required")]
    [Range(0, 20, ErrorMessage = "Max STS cranes must be between 0 and 20")]
    public int MaxSTS { get; set; } // Maximum number of STS cranes

    [Required(ErrorMessage = "At least one vessel type is required")]
    [MinLength(1, ErrorMessage = "At least one vessel type is required")]
    public List<Guid> AllowedVesselTypes { get; set; } // Allowed vessel types

    public List<string> STSCranes { get; set; } // List of STS crane IDs
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } // Creation date
    public DateTime? UpdatedAt { get; set; } // Last update date

    public static explicit operator DockDto(Dock entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var types = entity.AllowedVesselTypes.Select(vt => vt.Id).ToList();
        var cranes = entity.STSCranes.Select(vt => vt.Code.ToString()).ToList();

        return new DockDto
        {
            Id = entity.Id,
            Name = entity.Name.Value,
            Location = entity.Location.Value,
            Length = entity.Length.Value,
            Depth = entity.Depth.Value,
            MaxDraft = entity.MaxDraft.Value,
            MaxSTS = entity.MaxSTS.Value,
            AllowedVesselTypes = types,
            STSCranes = cranes,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

}
