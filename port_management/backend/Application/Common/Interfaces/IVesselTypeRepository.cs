using PortManagement.Domain.Vessel.Entities;

namespace PortManagement.Application.Common.Interfaces;

public interface IVesselTypeRepository
{
    Task<List<VesselType>> GetAllAsync(CancellationToken ct = default);
    Task<VesselType?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<VesselType?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<List<VesselType>> GetByIdsAsync(List<Guid> ids, CancellationToken ct = default);
    Task<List<VesselType>> SearchAsync(string searchTerm, CancellationToken ct = default);
    Task AddAsync(VesselType vesselType, CancellationToken ct = default);
    Task UpdateAsync(VesselType vesselType, CancellationToken ct = default);
    Task RemoveAsync(VesselType vesselType, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
