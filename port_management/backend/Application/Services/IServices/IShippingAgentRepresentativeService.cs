using PortManagement.Application.DTOs;

namespace PortManagement.Application.Services.IServices;

public interface IShippingAgentRepresentativeService
{
    Task<List<ShippingAgentRepresentativeDto>> GetAllAsync(CancellationToken ct = default);
    Task<ShippingAgentRepresentativeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<ShippingAgentRepresentativeDto>> GetByOrganizationIdAsync(string organizationId, CancellationToken ct = default);
    Task<ShippingAgentRepresentativeDto> CreateAsync(ShippingAgentRepresentativeDto dto, CancellationToken ct = default);
    Task<ShippingAgentRepresentativeDto?> UpdateAsync(int id, ShippingAgentRepresentativeDto dto,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
