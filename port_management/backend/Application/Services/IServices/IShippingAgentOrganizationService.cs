using PortManagement.Application.DTOs;

namespace PortManagement.Application.Services.IServices;

public interface IShippingAgentOrganizationService
{
    Task<List<ShippingAgentOrganizationDto>> GetAllAsync(CancellationToken ct = default);
    Task<ShippingAgentOrganizationDto?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<ShippingAgentOrganizationDto> CreateAsync(ShippingAgentOrganizationDto dto, CancellationToken ct = default);
    Task<ShippingAgentOrganizationDto?> UpdateAsync(string id, ShippingAgentOrganizationDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}
