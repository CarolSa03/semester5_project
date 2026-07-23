using PortManagement.Application.Services.IServices;
using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services;

public class PhysicalResourceService : IPhysicalResourceService
{
    private readonly IPhysicalResourceRepository _repository;

    public PhysicalResourceService(IPhysicalResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PhysicalResource>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<PhysicalResource?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
    public async Task<IEnumerable<T>> GetByTypeAsync<T>() where T : PhysicalResource
    {
        return await _repository.GetByTypeAsync<T>();
    }

    public async Task AddAsync(PhysicalResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        var existing = await _repository.GetByCodeAsync(resource.Code.Value);
        if (existing is not null)
            throw new InvalidOperationException($"A resource with code '{resource.Code.Value}' already exists.");

        await _repository.AddAsync(resource);
    }

    public async Task UpdateAsync(PhysicalResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        var existing = await _repository.GetByIdAsync(resource.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Resource with Id '{resource.Id}' not found.");

        await _repository.UpdateAsync(resource);
    }
    public async Task DeactivateAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Resource with Id '{id}' not found.");

        entity.MarkInactive();

        await _repository.UpdateAsync(entity);
    }
}
