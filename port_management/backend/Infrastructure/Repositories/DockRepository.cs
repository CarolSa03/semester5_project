using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories;

public class DockRepository : IDockRepository
{
    private readonly PortManagementDbContext _context;

    public DockRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dock>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Docks
            .Include(d => d.AllowedVesselTypes)
            .ToListAsync(ct);
    }

    public async Task<Dock?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Docks
            .Include(d => d.AllowedVesselTypes)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task<Dock?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Docks
            .Include(d => d.AllowedVesselTypes)
            .FirstOrDefaultAsync(d => d.Name == name, ct);
    }

    public async Task<List<Dock>> SearchAsync(string? name, string? location, Guid? vesselTypeId, CancellationToken ct = default)
    {
        var query = _context.Docks
            .Include(d => d.AllowedVesselTypes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(d => d.Name.Value.Contains(name));

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(d => d.Location.Value.Contains(location));

        if (vesselTypeId.HasValue)
            query = query.Where(d => d.AllowedVesselTypes.Any(vt => vt.Id == vesselTypeId.Value));

        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(Dock dock, CancellationToken ct = default)
    {
        await _context.Docks.AddAsync(dock, ct);
    }

    public async Task UpdateAsync(Dock dock, CancellationToken ct = default)
    {
        _context.Docks.Update(dock);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
