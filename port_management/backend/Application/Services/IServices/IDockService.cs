using PortManagement.Domain.Docks.Entities;

namespace PortManagement.Application.Services.IServices;

public interface IDockService
{
    Task<IEnumerable<Dock>> GetAllAsync(CancellationToken ct = default);
    Task<Dock?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Dock dock, CancellationToken ct = default);
    Task UpdateAsync(Dock update, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<List<Dock>> SearchAsync(string? name, string? location, Guid? vesselTypeId, CancellationToken ct = default);
}
