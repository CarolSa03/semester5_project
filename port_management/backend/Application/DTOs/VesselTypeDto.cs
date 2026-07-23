using System.ComponentModel.DataAnnotations;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Application.DTOs;

// Data Transfer Object for vessel type information
public class VesselTypeDto
{
    public Guid Id { get; set; } // Vessel type identifier (optional for creation)

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } // Name of the vessel type

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } // Description of the vessel type

    [Required(ErrorMessage = "Capacity TEU is required")]
    [Range(1, 50000, ErrorMessage = "Capacity TEU must be between 1 and 50,000")]
    public int CapacityTEU { get; set; } // Maximum capacity in TEU

    [Required(ErrorMessage = "Max rows is required")]
    [Range(1, 100, ErrorMessage = "Max rows must be between 1 and 100")]
    public int MaxRows { get; set; } // Maximum number of rows

    [Required(ErrorMessage = "Max bays is required")]
    [Range(1, 100, ErrorMessage = "Max bays must be between 1 and 100")]
    public int MaxBays { get; set; } // Maximum number of bays

    [Required(ErrorMessage = "Max tiers is required")]
    [Range(1, 100, ErrorMessage = "Max tiers must be between 1 and 100")]
    public int MaxTiers { get; set; } // Maximum number of tiers

    public bool IsActive { get; set; } // Indicates if the vessel type is active
    public DateTime CreatedAt { get; set; } // When the vessel type was created
    public DateTime? UpdatedAt { get; set; } // When the vessel type was last updated

    public static explicit operator VesselType(VesselTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new VesselType
        {
            Id = dto.Id,
            Name = new VTName(dto.Name),
            Description = new VTDescription(dto.Description),
            CapacityTEU = new VTCapacityTEU(dto.CapacityTEU),
            MaxRows = new VTMaxRows(dto.MaxRows),
            MaxBays = new VTMaxBays(dto.MaxBays),
            MaxTiers = new VTMaxTiers(dto.MaxTiers),
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
    public static explicit operator VesselTypeDto(VesselType entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new VesselTypeDto
        {
            Id = entity.Id,
            Name = entity.Name?.Value ?? string.Empty,
            Description = entity.Description?.Value ?? string.Empty,
            CapacityTEU = entity.CapacityTEU?.Value ?? 0,
            MaxRows = entity.MaxRows?.Value ?? 0,
            MaxBays = entity.MaxBays?.Value ?? 0,
            MaxTiers = entity.MaxTiers?.Value ?? 0,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
