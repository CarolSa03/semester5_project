using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories;

public class VesselRecordRepository : IVesselRecordRepository
{
    private readonly PortManagementDbContext _context;

    public VesselRecordRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<VesselRecord>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.VesselRecords
            .Include(v => v.VesselType)
            .ToListAsync(ct);
    }

    public async Task<VesselRecord?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.VesselRecords
            .Include(v => v.VesselType)
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<VesselRecord?> GetByIMOAsync(Imo imo, CancellationToken ct = default)
    {
        return await _context.VesselRecords
            .Include(v => v.VesselType)
            .FirstOrDefaultAsync(v => v.Imo == imo, ct);
    }

    public async Task<List<VesselRecord>> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
        var allRecords = await _context.VesselRecords
            .Include(v => v.VesselType)
            .Where(v =>
                v.Name.Value.Contains(searchTerm) ||
                v.Owner.Value.Contains(searchTerm))
            .ToListAsync(ct);

        if (int.TryParse(searchTerm, out var imoValue))
        {
            var imoMatches = await _context.VesselRecords
                .Include(v => v.VesselType)
                .Where(v => v.Imo == new Imo(imoValue))
                .ToListAsync(ct);

            allRecords = allRecords.Union(imoMatches).Distinct().ToList();
        }

        return allRecords;
    }

    public async Task<bool> ExistsByIMOAsync(Imo imo, CancellationToken ct = default)
    {
        return await _context.VesselRecords
            .AnyAsync(v => v.Imo == imo, ct);
    }

    public async Task AddAsync(VesselRecord vesselRecord, CancellationToken ct = default)
    {
        await _context.VesselRecords.AddAsync(vesselRecord, ct);
    }

    public async Task UpdateAsync(VesselRecord vesselRecord, CancellationToken ct = default)
    {
        _context.VesselRecords.Update(vesselRecord);
    }

    public async Task DeleteAsync(VesselRecord vesselRecord, CancellationToken ct = default)
    {
        _context.VesselRecords.Remove(vesselRecord);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Dictionary<Imo, string>> GetNamesByImosAsync(IEnumerable<Imo> imos, CancellationToken ct = default)
    {
        var imoList = imos.ToList();

        return await _context.VesselRecords
            .Where(v => imoList.Contains(v.Imo))
            .ToDictionaryAsync(v => v.Imo, v => v.Name.Value, ct);
    }
}
