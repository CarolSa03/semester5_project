using Microsoft.EntityFrameworkCore;
using PortManagement.Application.Common.Interfaces;
using PortManagement.Domain.Auth;
using PortManagement.Infrastructure.Data;


namespace PortManagement.Infrastructure.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly PortManagementDbContext _context;

        public AppUserRepository(PortManagementDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetByIdAsync(Guid id)
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AppUser?> GetByIdWithRolesAsync(Guid userId)
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }


        public async Task<AppUser?> GetByIamUserIdAsync(string iamUserId)
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.IamUserId == iamUserId);
        }

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AppUser?> GetByActivationTokenAsync(string activationToken)
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.ActivationToken == activationToken);
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetActiveUsersAsync()
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetInactiveUsersAsync()
        {
            return await _context.AppUsers
                .Include(u => u.Roles)
                .Where(u => !u.IsActive)
                .ToListAsync();
        }

        public async Task<AppUser> AddAsync(AppUser user)
        {
            await _context.AppUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<AppUser> UpdateAsync(AppUser user)
        {
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task AddRoleAsync(AppUser user, Role role, string assignedBy)
        {
            if (_context.Entry(user).State == EntityState.Detached)
            {
                _context.AppUsers.Attach(user);
            }

            if (!user.HasRole(role))
            {
                var userRole = UserRole.Create(user.Id, role, assignedBy);
                await _context.Set<UserRole>().AddAsync(userRole);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.AppUsers.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByIamUserIdAsync(string iamUserId)
        {
            return await _context.AppUsers.AnyAsync(u => u.IamUserId == iamUserId);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.AppUsers.AnyAsync(u => u.Email == email);
        }
    }
}