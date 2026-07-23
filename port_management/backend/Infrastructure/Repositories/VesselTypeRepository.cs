using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories;

public class VesselTypeRepository : IVesselTypeRepository
{
    private readonly PortManagementDbContext _context;

    public VesselTypeRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<VesselType>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.VesselTypes.ToListAsync(ct);
    }

    public async Task<VesselType?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.VesselTypes.FirstOrDefaultAsync(vt => vt.Id == id, ct);
    }

    public async Task<VesselType?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.VesselTypes.FirstOrDefaultAsync(vt => vt.Name == name, ct);
    }

    public async Task<List<VesselType>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default)
    {
        return await _context.VesselTypes
            .Where(vt => ids.Contains(vt.Id))
            .ToListAsync(ct);
    }

    public async Task<List<VesselType>> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
        var normalized = searchTerm.Trim().ToLower();
        return await _context.VesselTypes
            .Where(vt =>
                vt.Name.Value.ToLower().Contains(normalized) ||
                vt.Description.Value.ToLower().Contains(normalized))
            .ToListAsync(ct);
    }

    public async Task AddAsync(VesselType vesselType, CancellationToken ct = default)
    {
        await _context.VesselTypes.AddAsync(vesselType, ct);
    }

    public async Task UpdateAsync(VesselType vesselType, CancellationToken ct = default)
    {
        _context.VesselTypes.Update(vesselType);
    }

    public async Task RemoveAsync(VesselType vesselType, CancellationToken ct = default)
    {
        _context.VesselTypes.Remove(vesselType);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
