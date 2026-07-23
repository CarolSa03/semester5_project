using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Application.Common.Interfaces;

public interface IVesselRecordRepository
{
    Task<List<VesselRecord>> GetAllAsync(CancellationToken ct = default);
    Task<VesselRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<VesselRecord?> GetByIMOAsync(Imo imo, CancellationToken ct = default);
    Task<List<VesselRecord>> SearchAsync(string searchTerm, CancellationToken ct = default);
    Task<bool> ExistsByIMOAsync(Imo imo, CancellationToken ct = default);
    Task AddAsync(VesselRecord vesselRecord, CancellationToken ct = default);
    Task UpdateAsync(VesselRecord vesselRecord, CancellationToken ct = default);
    Task DeleteAsync(VesselRecord vesselRecord, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<Dictionary<Imo, string>> GetNamesByImosAsync(IEnumerable<Imo> imos, CancellationToken ct = default);
}
