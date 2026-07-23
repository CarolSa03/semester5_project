using PortManagement.Domain.PhysicalResources.Entities;

namespace PortManagement.Application.Services.IServices;

public interface IPhysicalResourceService
{
    Task<IEnumerable<PhysicalResource>> GetAllAsync();
    Task<PhysicalResource?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetByTypeAsync<T>() where T : PhysicalResource;
    Task AddAsync(PhysicalResource resource);
    Task UpdateAsync(PhysicalResource resource);
    Task DeactivateAsync(Guid id);
}
