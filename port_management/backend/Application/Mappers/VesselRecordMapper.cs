using PortManagement.Application.DTOs;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Domain.Vessel.Enums;

namespace PortManagement.Application.Mappers;

public class VesselRecordMapper
{
    // Map DTO to Entity
    public static VesselRecord ToEntity(VesselRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (!Imo.TryCreate(dto.ImoValue, out var imo))
            throw new ArgumentException($"Invalid IMO format: {dto.ImoValue}");

        var vesselName = new VRName(dto.Name);
        var vesselOwner = new VROwner(dto.Owner);
        var vesselType = new VesselType { Id = dto.VesselTypeId };
        var status = dto.IsActive ? VRStatus.Active : VRStatus.Inactive;

        return new VesselRecord(imo, vesselName, vesselType, vesselOwner, status);
    }

    // Map Entity to DTO
    public static VesselRecordDto ToDto(VesselRecord entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new VesselRecordDto
        {
            Id = entity.Id,
            ImoValue = entity.Imo.ToString(),
            Name = entity.Name.Value,
            VesselTypeId = entity.VesselType.Id,
            Owner = entity.Owner.Value,
            IsActive = entity.Status == VRStatus.Active,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
