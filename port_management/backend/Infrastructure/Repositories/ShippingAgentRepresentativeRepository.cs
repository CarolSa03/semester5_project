using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories;

public class ShippingAgentRepresentativeRepository : IShippingAgentRepresentativeRepository
{
    private readonly PortManagementDbContext _context;

    public ShippingAgentRepresentativeRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ShippingAgentRepresentative?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.ShippingAgentRepresentatives
            .Include(r => r.ShippingAgentOrganization)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<List<ShippingAgentRepresentative>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.ShippingAgentRepresentatives
            .Include(r => r.ShippingAgentOrganization)
            .ToListAsync(ct);
    }

    public async Task<List<ShippingAgentRepresentative>> GetByOrganizationIdAsync(string organizationId, CancellationToken ct = default)
    {
        return await _context.ShippingAgentRepresentatives
            .Include(r => r.ShippingAgentOrganization)
            .Where(r => r.ShippingAgentOrganizationId == organizationId)
            .ToListAsync(ct);
    }

    public async Task<ShippingAgentRepresentative?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.ShippingAgentRepresentatives
            .Include(r => r.ShippingAgentOrganization)
            .FirstOrDefaultAsync(r => r.Email == email, ct);
    }

    public async Task AddAsync(ShippingAgentRepresentative representative, CancellationToken ct = default)
    {
        await _context.ShippingAgentRepresentatives.AddAsync(representative, ct);
    }

    public async Task UpdateAsync(ShippingAgentRepresentative representative, CancellationToken ct = default)
    {
        _context.ShippingAgentRepresentatives.Update(representative);
    }

    public async Task DeleteAsync(ShippingAgentRepresentative representative, CancellationToken ct = default)
    {
        _context.ShippingAgentRepresentatives.Remove(representative);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<string?> GetOrganizationIdAsync(int representativeId, CancellationToken ct = default)
    {
        var representative = await _context.ShippingAgentRepresentatives
            .FirstOrDefaultAsync(r => r.Id == representativeId, ct);

        return representative?.ShippingAgentOrganizationId;
    }

    public async Task<List<ShippingAgentRepresentative>> GetByOrganizationAsync(string organizationId, CancellationToken ct = default)
    {
        return await _context.ShippingAgentRepresentatives
            .Where(r => r.ShippingAgentOrganizationId == organizationId)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<string> ids, CancellationToken ct = default)
    {
        var representatives = await _context.ShippingAgentRepresentatives
            .Where(r => ids.Contains(r.Id.ToString()))
            .ToListAsync(ct);

        return representatives.ToDictionary(r => r.Id, r => r.Name);
    }
}
