using PortManagement.Domain.Privacy;

namespace PortManagement.Application.Services.IServices
{
    public interface IPrivacyPolicyService
    {
        Task<PrivacyPolicy?> GetCurrentAsync();
        Task<IEnumerable<PrivacyPolicy>> GetAllAsync();
        Task CreateAsync(string content, string createdBy);
        Task<bool> NeedsNotificationAsync(string userId);
        Task MarkAsViewedAsync(string userId);
    }
}
