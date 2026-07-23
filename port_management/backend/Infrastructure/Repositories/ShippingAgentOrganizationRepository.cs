using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories;

public class ShippingAgentOrganizationRepository : IShippingAgentOrganizationRepository
{
    private readonly PortManagementDbContext _context;

    public ShippingAgentOrganizationRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ShippingAgentOrganization?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await _context.ShippingAgentOrganizations
            .Include(o => o.Representatives)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<List<ShippingAgentOrganization>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.ShippingAgentOrganizations
            .Include(o => o.Representatives)
            .ToListAsync(ct);
    }

    public async Task<ShippingAgentOrganization?> GetByTaxNumberAsync(string taxNumber, CancellationToken ct = default)
    {
        return await _context.ShippingAgentOrganizations
            .Include(o => o.Representatives)
            .FirstOrDefaultAsync(o => o.TaxNumber == taxNumber, ct);
    }

    public async Task AddAsync(ShippingAgentOrganization organization, CancellationToken ct = default)
    {
        await _context.ShippingAgentOrganizations.AddAsync(organization, ct);
    }

    public async Task UpdateAsync(ShippingAgentOrganization organization, CancellationToken ct = default)
    {
        _context.ShippingAgentOrganizations.Update(organization);
    }

    public async Task DeleteAsync(ShippingAgentOrganization organization, CancellationToken ct = default)
    {
        _context.ShippingAgentOrganizations.Remove(organization);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
