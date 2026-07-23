using PortManagement.Domain.Storage.Entities;

namespace PortManagement.Application.Common.Interfaces
{
    public interface IStorageAreaRepository
    {
        Task<List<StorageArea>> GetAllAsync();
        Task<StorageArea?> GetByIdAsync(Guid id);
        Task<StorageArea?> GetByBusinessIdAsync(string businessId);
        Task AddAsync(StorageArea entity);
        Task UpdateAsync(StorageArea entity);
        Task RemoveAsync(StorageArea entity);
        Task SaveChangesAsync();
    }
}
