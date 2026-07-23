using PortManagement.Application.Common.Interfaces;
using PortManagement.Application.DTOs.Auth;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Auth;

namespace PortManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public AuthService(IAppUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
        }

        //3.2.2
        public async Task<UserAuthorizationDto> GetUserAuthorizationAsync(string iamUserId)
        {
            var user = await _userRepository.GetByIamUserIdAsync(iamUserId);

            if (user == null)
                throw new InvalidOperationException($"User with IAM ID '{iamUserId}' not found in internal system");

            if (!user.IsActive)
                throw new InvalidOperationException("User account is not active");

            return AuthMapper.ToAuthorizationDto(user);
        }

        //3.2.5
        public async Task<AppUserDto> CreateUserForActivationAsync(CreateAppUserDto dto, string createdBy)
        {
            if (await _userRepository.ExistsByIamUserIdAsync(dto.IamUserId))
                throw new InvalidOperationException($"User with IAM ID '{dto.IamUserId}' already exists");

            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException($"Email '{dto.Email}' is already in use");

            var user = AppUser.CreateForActivation(
                dto.IamUserId,
                dto.Email,
                dto.Name,
                TimeSpan.FromDays(7)
            );

            await _userRepository.AddAsync(user);

            await _auditService.LogUserManagementEventAsync(
                AuditEventType.UserCreated,
                createdBy,
                user.IamUserId,
                $"User '{user.Name}' created with email '{user.Email}'. Activation token generated."
            );

            return AuthMapper.ToDto(user);
        }

        //3.2.1
        public async Task<AppUserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found");

            return AuthMapper.ToDto(user);
        }

        //3.2.1
        public async Task<IEnumerable<AppUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return AuthMapper.ToDtoList(users);
        }
        public async Task<AppUserDto> UpdateUserProfileAsync(Guid userId, UpdateAppUserDto dto, string updatedBy)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found");

            if (user.Email != dto.Email && await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException($"Email '{dto.Email}' is already in use");

            user.UpdateProfile(dto.Email, dto.Name);
            await _userRepository.UpdateAsync(user);

            await _auditService.LogUserManagementEventAsync(
                AuditEventType.UserUpdated,
                updatedBy,
                user.IamUserId,
                $"User profile updated. Name: '{dto.Name}', Email: '{dto.Email}'"
            );

            return AuthMapper.ToDto(user);
        }

        //3.2.5
        public async Task<AppUserDto> AssignRoleToUserAsync(Guid userId, Role role, string assignedBy)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found");

            await _userRepository.AddRoleAsync(user, role, assignedBy);

            await _auditService.LogRoleManagementEventAsync(
                AuditEventType.RoleAssigned,
                assignedBy,
                user.IamUserId,
                role.ToString()
            );

            return AuthMapper.ToDto(user);
        }

        //3.2.5
        public async Task<AppUserDto> RemoveRoleFromUserAsync(Guid userId, Role role, string removedBy)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found");

            user.RemoveRole(role);
            await _userRepository.UpdateAsync(user);

            await _auditService.LogRoleManagementEventAsync(
                AuditEventType.RoleRemoved,
                removedBy,
                user.IamUserId,
                role.ToString()
            );

            return AuthMapper.ToDto(user);
        }

        //3.2.5
        public async Task<AppUserDto> DeactivateUserAsync(Guid userId, string deactivatedBy)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found");

            user.Deactivate();
            await _userRepository.UpdateAsync(user);

            await _auditService.LogUserManagementEventAsync(
                AuditEventType.UserDeactivated,
                deactivatedBy,
                user.IamUserId,
                $"User '{user.Name}' deactivated by administrator"
            );

            return AuthMapper.ToDto(user);
        }

        //3.2.5
        public async Task<AppUserDto> ActivateUserAccountAsync(Guid userId, string activatedBy)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found");

            if (user.IsActive)
                throw new InvalidOperationException("User is already active");

            user.Activate();
            await _userRepository.UpdateAsync(user);

            await _auditService.LogUserManagementEventAsync(
                AuditEventType.UserActivated,
                activatedBy,
                user.IamUserId,
                $"User '{user.Name}' activated by administrator"
            );

            return AuthMapper.ToDto(user);
        }

        //3.2.6
        public async Task<AppUserDto> ActivateUserWithTokenAsync(string activationToken, string iamUserId)
        {
            var user = await _userRepository.GetByActivationTokenAsync(activationToken);

            if (user == null)
                throw new InvalidOperationException("Invalid activation token");

            if (user.IamUserId != iamUserId)
                throw new InvalidOperationException("Activation token does not match authenticated user");

            user.Activate();
            await _userRepository.UpdateAsync(user);

            await _auditService.LogUserManagementEventAsync(
                AuditEventType.UserActivated,
                user.IamUserId,
                user.IamUserId,
                $"User '{user.Name}' activated their account successfully"
            );

            return AuthMapper.ToDto(user);
        }

        //3.2.4
        public async Task<bool> CheckAuthorizationAsync(string iamUserId, params Role[] requiredRoles)
        {
            var user = await _userRepository.GetByIamUserIdAsync(iamUserId);

            if (user == null)
                return false;

            return user.CanPerformAction(requiredRoles);
        }

        public async Task<bool> IsUserActiveAsync(string iamUserId)
        {
            var user = await _userRepository.GetByIamUserIdAsync(iamUserId);
            return user?.IsActive ?? false;
        }
        public async Task<AppUser?> GetUserByIamIdAsync(string iamUserId)
        {
            return await _userRepository.GetByIamUserIdAsync(iamUserId);
        }

        public async Task<AppUserDto> RegenerateActivationTokenAsync(string iamUserId, string requestedBy)
        {
            var user = await _userRepository.GetByIamUserIdAsync(iamUserId);
            if (user == null)
                throw new InvalidOperationException($"User with IAM ID {iamUserId} not found");
            if (user.IsActive)
                throw new InvalidOperationException("User account is already active");

            user.RegenerateActivationToken(TimeSpan.FromDays(7));

            await _userRepository.UpdateAsync(user);

            await _auditService.LogUserManagementEventAsync(
                AuditEventType.UserUpdated,
                requestedBy,
                user.IamUserId,
                $"Activation token regenerated for user {user.Name}"
            );

            return AuthMapper.ToDto(user);
        }

        public async Task<AppUserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return AuthMapper.ToDto(user);
        }
    }
}