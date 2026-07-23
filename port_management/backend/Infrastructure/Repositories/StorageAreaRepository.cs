using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Storage.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories;

public class StorageAreaRepository : IStorageAreaRepository
{
    private readonly PortManagementDbContext _context;

    public StorageAreaRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<StorageArea>> GetAllAsync()
    {
        return await _context.StorageAreas
            .Include(s => s.ServedDocks)
            .ToListAsync();
    }

    public async Task<StorageArea?> GetByIdAsync(Guid id)
    {
        return await _context.StorageAreas
            .Include(s => s.ServedDocks)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<StorageArea?> GetByBusinessIdAsync(string businessId)
    {
        return await _context.StorageAreas
            .Include(s => s.ServedDocks)
            .FirstOrDefaultAsync(s => s.BusinessId.Value == businessId);
    }

    public async Task AddAsync(StorageArea entity)
    {
        await _context.StorageAreas.AddAsync(entity);
    }

    public async Task UpdateAsync(StorageArea entity)
    {
        _context.StorageAreas.Update(entity);
    }

    public async Task RemoveAsync(StorageArea entity)
    {
        _context.StorageAreas.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
