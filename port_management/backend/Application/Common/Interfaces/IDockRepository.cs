using PortManagement.Domain.Docks.Entities;

namespace PortManagement.Application.Common.Interfaces;

public interface IDockRepository
{
    Task<List<Dock>> GetAllAsync(CancellationToken ct = default);
    Task<Dock?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Dock?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<List<Dock>> SearchAsync(string? name, string? location, Guid? vesselTypeId, CancellationToken ct = default);
    Task AddAsync(Dock dock, CancellationToken ct = default);
    Task UpdateAsync(Dock dock, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
