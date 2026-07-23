using PortManagement.Domain.ShippingAgent.Entities;

namespace PortManagement.Application.Common.Interfaces;

public interface IShippingAgentOrganizationRepository
{
    Task<ShippingAgentOrganization?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<ShippingAgentOrganization>> GetAllAsync(CancellationToken ct = default);
    Task<ShippingAgentOrganization?> GetByTaxNumberAsync(string taxNumber, CancellationToken ct = default);
    Task AddAsync(ShippingAgentOrganization organization, CancellationToken ct = default);
    Task UpdateAsync(ShippingAgentOrganization organization, CancellationToken ct = default);
    Task DeleteAsync(ShippingAgentOrganization organization, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
