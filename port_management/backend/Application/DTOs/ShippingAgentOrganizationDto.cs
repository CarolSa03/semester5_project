using PortManagement.Domain.ShippingAgent.Entities;

namespace PortManagement.Application.DTOs;

public class ShippingAgentOrganizationDto
{
    public string Id { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string AlternativeName { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static explicit operator ShippingAgentOrganizationDto(ShippingAgentOrganization entity)
    {
        return new ShippingAgentOrganizationDto
        {
            Id = entity.Id,
            LegalName = entity.LegalName,
            AlternativeName = entity.AlternativeName,
            TaxNumber = entity.TaxNumber,
            CreatedAt = entity.CreatedAt
        };
    }
    public static explicit operator ShippingAgentOrganization(ShippingAgentOrganizationDto dto)
    {
        return new ShippingAgentOrganization
        {
            Id = dto.Id,
            LegalName = dto.LegalName,
            AlternativeName = dto.AlternativeName,
            TaxNumber = dto.TaxNumber,
            CreatedAt = dto.CreatedAt
        };
    }
}
