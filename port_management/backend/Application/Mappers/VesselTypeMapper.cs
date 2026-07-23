using PortManagement.Application.DTOs;
using PortManagement.Domain.Vessel.Entities;

namespace PortManagement.Application.Mappers;

public class VesselTypeMapper
{
    // Map entity to DTO (reusable method)
    public static VesselTypeDto ToDto(VesselType entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        return entity switch
        {
            VesselType vesselType => (VesselTypeDto)vesselType,
            _ => throw new ArgumentException("Unknown entity type", nameof(entity))
        };
    }
    public static VesselType ToEntity(VesselTypeDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return dto switch
        {
            VesselTypeDto vesselTypeDto => (VesselType)vesselTypeDto,
            _ => throw new ArgumentException("Unknown DTO type", nameof(dto))
        };
    }
}
