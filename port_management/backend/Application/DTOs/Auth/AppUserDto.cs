using PortManagement.Domain.Auth;

namespace PortManagement.Application.DTOs.Auth
{
    public class AppUserDto
    {
        public Guid Id { get; set; }
        public string IamUserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Roles { get; set; }
        public string? ActivationToken { get; set; }
    }

    public class CreateAppUserDto
    {
        public string IamUserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class AssignRoleDto
    {
        public Role Role { get; set; }
    }

    public class UpdateAppUserDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class ActivateUserDto
    {
        public string ActivationToken { get; set; }
        public string IamUserId { get; set; }
    }

    public class UserAuthorizationDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<Role> Roles { get; set; }
        public bool IsActive { get; set; }
    }
}
