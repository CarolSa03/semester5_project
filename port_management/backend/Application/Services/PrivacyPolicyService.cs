using PortManagement.Application.Common.Interfaces;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Privacy;

namespace PortManagement.Application.Services
{
    public class PrivacyPolicyService : IPrivacyPolicyService
    {
        private readonly IPrivacyPolicyRepository _repository;

        public PrivacyPolicyService(IPrivacyPolicyRepository repository)
        {
            _repository = repository;
        }

        public async Task<PrivacyPolicy?> GetCurrentAsync()
        {
            return await _repository.GetCurrentAsync();
        }

        public async Task<IEnumerable<PrivacyPolicy>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task CreateAsync(string content, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be empty");
            }

            await _repository.DeactivateAllAsync();

            var version = await _repository.GetLatestVersionAsync();

            var policy = new PrivacyPolicy
            {
                Id = Guid.NewGuid(),
                Content = content,
                Version = version + 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                IsActive = true
            };

            await _repository.AddAsync(policy);
        }

        public async Task<bool> NeedsNotificationAsync(string userId)
        {
            var current = await _repository.GetCurrentAsync();
            if (current == null) return false;

            var viewed = await _repository.HasUserViewedAsync(userId, current.Version);
            return !viewed;
        }

        public async Task MarkAsViewedAsync(string userId)
        {
            var current = await _repository.GetCurrentAsync();
            if (current == null)
            {
                throw new InvalidOperationException("No active policy");
            }

            await _repository.AddUserViewAsync(userId, current.Version);
        }
    }
}
