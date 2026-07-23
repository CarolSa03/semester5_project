using System.Diagnostics;
using PortManagement.Application.DTOs;
using PortManagement.Domain.ShippingAgent.Entities;

namespace PortManagement.Application.Mappers;

public class ShippingAgentOrganizationMapper
{
    public static ShippingAgentOrganizationDto ToDto(ShippingAgentOrganization entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        return entity switch
        {
            ShippingAgentOrganization shippingAgentOrganization => (ShippingAgentOrganizationDto)shippingAgentOrganization,
            _ => throw new ArgumentException("Unknown entity type", nameof(entity))
        };
    }
    public static ShippingAgentOrganization ToEntity(ShippingAgentOrganizationDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return dto switch
        {
            ShippingAgentOrganizationDto shippingAgentOrganizationDto => (ShippingAgentOrganization)shippingAgentOrganizationDto,
            _ => throw new ArgumentException("Unknown entity type", nameof(dto))
        };
    }
}
