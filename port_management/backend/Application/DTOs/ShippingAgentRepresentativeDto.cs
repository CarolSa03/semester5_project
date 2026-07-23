using PortManagement.Domain.ShippingAgent.Entities;

namespace PortManagement.Application.DTOs;

public class ShippingAgentRepresentativeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ShippingAgentOrganizationId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static explicit operator ShippingAgentRepresentativeDto(ShippingAgentRepresentative entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new ShippingAgentRepresentativeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            ShippingAgentOrganizationId = entity.ShippingAgentOrganizationId,
            CreatedAt = entity.CreatedAt
        };
    }
    public static explicit operator ShippingAgentRepresentative(ShippingAgentRepresentativeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ShippingAgentRepresentative
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            ShippingAgentOrganizationId = dto.ShippingAgentOrganizationId,
            CreatedAt = dto.CreatedAt
        };
    }


}
