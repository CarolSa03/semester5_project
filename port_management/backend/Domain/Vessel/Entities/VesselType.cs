using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Domain.Vessel.Entities;

public class VesselType
{
    public Guid Id { get; set; }

    public VTName Name { get; set; } = default!;

    public VTDescription Description { get; set; } = default!;

    public VTCapacityTEU CapacityTEU { get; set; } = default!;

    public VTMaxRows MaxRows { get; set; } = default!;

    public VTMaxBays MaxBays { get; set; } = default!;

    public VTMaxTiers MaxTiers { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

}
