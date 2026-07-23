using PortManagement.Domain.ShippingAgent.Entities;

namespace PortManagement.Application.Common.Interfaces;

public interface IShippingAgentRepresentativeRepository
{
    Task<ShippingAgentRepresentative?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<ShippingAgentRepresentative>> GetAllAsync(CancellationToken ct = default);
    Task<List<ShippingAgentRepresentative>> GetByOrganizationIdAsync(string organizationId, CancellationToken ct = default);
    Task<ShippingAgentRepresentative?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(ShippingAgentRepresentative representative, CancellationToken ct = default);
    Task UpdateAsync(ShippingAgentRepresentative representative, CancellationToken ct = default);
    Task DeleteAsync(ShippingAgentRepresentative representative, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<string?> GetOrganizationIdAsync(int representativeId, CancellationToken ct = default);
    Task<List<ShippingAgentRepresentative>> GetByOrganizationAsync(string organizationId, CancellationToken ct = default);
    Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<string> ids, CancellationToken ct = default);
}
