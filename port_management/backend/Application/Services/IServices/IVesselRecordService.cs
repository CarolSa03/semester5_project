using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Application.Services.IServices;

public interface IVesselRecordService
{
    Task<IEnumerable<VesselRecord>> GetAllAsync(CancellationToken ct = default);
    Task<VesselRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<VesselRecord?> GetByIMOAsync(Imo imo, CancellationToken ct = default);
    Task<IEnumerable<VesselRecord>> SearchAsync(string searchTerm, CancellationToken ct = default);
    Task AddAsync(VesselRecord vesselRecord, CancellationToken ct = default);
    Task UpdateAsync(VesselRecord vesselRecord, CancellationToken ct = default);
    Task InactivateAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

