using PortManagement.Application.DTOs;
using PortManagement.Domain.ShippingAgent.Entities;

namespace PortManagement.Application.Mappers;

public class ShippingAgentRepresentativeMapper
{
    public static ShippingAgentRepresentativeDto ToDto(ShippingAgentRepresentative entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        return entity switch
        {
            ShippingAgentRepresentative shippingAgentRepresentative => (ShippingAgentRepresentativeDto)shippingAgentRepresentative,
            _ => throw new ArgumentException("Unknown entity type", nameof(entity))
        };
    }

    public static ShippingAgentRepresentative ToEntity(ShippingAgentRepresentativeDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return dto switch
        {
            ShippingAgentRepresentativeDto shippingAgentRepresentativeDto => (ShippingAgentRepresentative)shippingAgentRepresentativeDto,
            _ => throw new ArgumentException("Unknown entity type", nameof(dto))
        };
    }
}
