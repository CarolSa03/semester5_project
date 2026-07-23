using PortManagement.Domain.Storage.Entities;

namespace PortManagement.Application.Services.IServices;

public interface IStorageAreaService
{
    Task<IEnumerable<StorageArea>> GetAllAsync();
    Task<StorageArea?> GetByIdAsync(Guid id);
    Task<StorageArea?> GetByBusinessIdAsync(string businessId);
    Task AddAsync(StorageArea storageArea);
    Task UpdateAsync(StorageArea storageArea);
    Task DeleteAsync(Guid id);
}

