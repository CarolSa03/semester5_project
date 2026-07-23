using PortManagement.Domain.Privacy;

namespace PortManagement.Application.Common.Interfaces
{
    public interface IPrivacyPolicyRepository
    {
        Task<PrivacyPolicy?> GetCurrentAsync();
        Task<IEnumerable<PrivacyPolicy>> GetAllAsync();
        Task AddAsync(PrivacyPolicy policy);
        Task<int> GetLatestVersionAsync();
        Task DeactivateAllAsync();
        Task<bool> HasUserViewedAsync(string userId, int version);
        Task AddUserViewAsync(string userId, int version);
    }
}
