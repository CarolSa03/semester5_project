using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Storage.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services
{
    public class StorageAreaServices : IStorageAreaService
    {
        private readonly IStorageAreaRepository _repository;

        public StorageAreaServices(IStorageAreaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StorageArea>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<StorageArea?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<StorageArea?> GetByBusinessIdAsync(string businessId)
        {
            return await _repository.GetByBusinessIdAsync(businessId);
        }

        public async Task AddAsync(StorageArea storageArea)
        {
            ArgumentNullException.ThrowIfNull(storageArea);

            var existing = await _repository.GetByBusinessIdAsync(storageArea.BusinessId.Value);
            if (existing != null)
                throw new InvalidOperationException($"Storage area with BusinessId '{storageArea.BusinessId.Value}' already exists.");

            await _repository.AddAsync(storageArea);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(StorageArea storageArea)
        {
            ArgumentNullException.ThrowIfNull(storageArea);

            var existing = await _repository.GetByIdAsync(storageArea.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Storage area with Id '{storageArea.Id}' not found.");

            await _repository.UpdateAsync(storageArea);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Storage area with Id '{id}' not found.");

            await _repository.RemoveAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }
}

