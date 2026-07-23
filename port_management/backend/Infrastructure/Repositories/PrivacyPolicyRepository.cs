using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Privacy;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories
{
    public class PrivacyPolicyRepository : IPrivacyPolicyRepository
    {
        private readonly PortManagementDbContext _context;

        public PrivacyPolicyRepository(PortManagementDbContext context)
        {
            _context = context;
        }

        public async Task<PrivacyPolicy?> GetCurrentAsync()
        {
            return await _context.PrivacyPolicies
                .FirstOrDefaultAsync(p => p.IsActive);
        }

        public async Task<IEnumerable<PrivacyPolicy>> GetAllAsync()
        {
            return await _context.PrivacyPolicies
                .OrderByDescending(p => p.Version)
                .ToListAsync();
        }

        public async Task AddAsync(PrivacyPolicy policy)
        {
            await _context.PrivacyPolicies.AddAsync(policy);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetLatestVersionAsync()
        {
            var latest = await _context.PrivacyPolicies
                .OrderByDescending(p => p.Version)
                .FirstOrDefaultAsync();

            return latest?.Version ?? 0;
        }

        public async Task DeactivateAllAsync()
        {
            var active = await _context.PrivacyPolicies
                .Where(p => p.IsActive)
                .ToListAsync();

            foreach (var policy in active)
            {
                policy.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasUserViewedAsync(string userId, int version)
        {
            return await _context.UserPolicyViews
                .AnyAsync(v => v.UserId == userId && v.PolicyVersion == version);
        }

        public async Task AddUserViewAsync(string userId, int version)
        {
            var view = new UserPolicyView
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PolicyVersion = version,
                ViewedAt = DateTime.UtcNow
            };

            await _context.UserPolicyViews.AddAsync(view);
            await _context.SaveChangesAsync();
        }
    }
}
