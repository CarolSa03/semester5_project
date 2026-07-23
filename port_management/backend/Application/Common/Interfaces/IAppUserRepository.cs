using PortManagement.Domain.Auth;

namespace PortManagement.Application.Common.Interfaces
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetByIdAsync(Guid id);
        Task<AppUser?> GetByIamUserIdAsync(string iamUserId);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByActivationTokenAsync(string activationToken);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<IEnumerable<AppUser>> GetActiveUsersAsync();
        Task<IEnumerable<AppUser>> GetInactiveUsersAsync();
        Task<AppUser> AddAsync(AppUser user);
        Task<AppUser> UpdateAsync(AppUser user);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByIamUserIdAsync(string iamUserId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<AppUser?> GetByIdWithRolesAsync(Guid userId);
        Task AddRoleAsync(AppUser user, Role role, string assignedBy);
    }
}