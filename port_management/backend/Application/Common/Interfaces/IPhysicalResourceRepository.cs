using PortManagement.Domain.PhysicalResources.Entities;

namespace PortManagement.Application.Common.Interfaces;

public interface IPhysicalResourceRepository
{
    Task AddAsync(PhysicalResource resource);
    Task UpdateAsync(PhysicalResource resource);
    Task<PhysicalResource?> GetByIdAsync(Guid id);
    Task<PhysicalResource?> GetByCodeAsync(string code);
    Task<IEnumerable<PhysicalResource>> GetAllAsync();
    Task<IEnumerable<T>> GetByTypeAsync<T>() where T : PhysicalResource;
}
