using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories
{
    public class PhysicalResourceRepository : IPhysicalResourceRepository
    {
        private readonly PortManagementDbContext _context;

        public PhysicalResourceRepository(PortManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhysicalResource>> GetAllAsync()
        {
            return await _context.PhysicalResources
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PhysicalResource?> GetByIdAsync(Guid id)
        {
            return await _context.PhysicalResources
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PhysicalResource?> GetByCodeAsync(string code)
        {
            return await _context.PhysicalResources
                .FirstOrDefaultAsync(p => p.Code.Value == code);
        }

        public async Task AddAsync(PhysicalResource resource)
        {
            await _context.PhysicalResources.AddAsync(resource);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PhysicalResource resource)
        {
            var local = _context.PhysicalResources.Local
                .FirstOrDefault(entry => entry.Id.Equals(resource.Id));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.PhysicalResources.Update(resource);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<T>> GetByTypeAsync<T>() where T : PhysicalResource
        {
            return _context.PhysicalResources
                .OfType<T>()
                .AsNoTracking()
                .ToListAsync()
                .ContinueWith(t => (IEnumerable<T>)t.Result);
        }
    }
}
