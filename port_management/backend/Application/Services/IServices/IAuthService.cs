using PortManagement.Application.DTOs.Auth;
using PortManagement.Domain.Auth;

namespace PortManagement.Application.Services.IServices
{
    public interface IAuthService
    {
        //3.2.2
        Task<UserAuthorizationDto> GetUserAuthorizationAsync(string iamUserId);

        //3.2.1 & 3.2.5
        Task<AppUserDto> CreateUserForActivationAsync(CreateAppUserDto dto, string createdBy);
        Task<AppUserDto> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<AppUserDto>> GetAllUsersAsync();
        Task<AppUserDto> UpdateUserProfileAsync(Guid userId, UpdateAppUserDto dto, string updatedBy);

        //3.2.5
        Task<AppUserDto> AssignRoleToUserAsync(Guid userId, Role role, string assignedBy);
        Task<AppUserDto> RemoveRoleFromUserAsync(Guid userId, Role role, string removedBy);

        //3.2.5
        Task<AppUserDto> DeactivateUserAsync(Guid userId, string deactivatedBy);
        Task<AppUserDto> ActivateUserAccountAsync(Guid userId, string activatedBy);

        //3.2.6
        Task<AppUserDto> ActivateUserWithTokenAsync(string activationToken, string iamUserId);

        //3.2.4
        Task<bool> CheckAuthorizationAsync(string iamUserId, params Role[] requiredRoles);
        Task<bool> IsUserActiveAsync(string iamUserId);
        Task<AppUser?> GetUserByIamIdAsync(string iamUserId);
        Task<AppUserDto> RegenerateActivationTokenAsync(string iamUserId, string requestedBy);
        Task<AppUserDto> GetUserByEmailAsync(string email);
    }
}