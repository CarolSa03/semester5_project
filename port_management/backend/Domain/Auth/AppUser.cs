using System.Security.Cryptography;
namespace PortManagement.Domain.Auth

{
    public class AppUser
    {
        public Guid Id { get; private set; }
        public string IamUserId { get; private set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; set; }
        public string ActivationToken { get; private set; }
        public DateTime? ActivationTokenExpiry { get; private set; }
        public virtual ICollection<UserRole> Roles { get; private set; } = new List<UserRole>();
        private AppUser()
        {
            Roles = new List<UserRole>();
        }
        public static AppUser CreateForActivation(string iamUserId, string email, string name, TimeSpan tokenExpiry)
        {
            if (string.IsNullOrWhiteSpace(iamUserId))
                throw new ArgumentException("IAM User ID is required", nameof(iamUserId));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                IamUserId = iamUserId,
                Email = email,
                Name = name,
                IsActive = false,
                ActivationToken = GenerateSecureToken(),
                ActivationTokenExpiry = DateTime.UtcNow.Add(tokenExpiry),
                CreatedAt = DateTime.UtcNow
            };

            return user;
        }
        public void Activate()
        {
            if (!IsActivationTokenValid())
                throw new InvalidOperationException("Activation token is invalid or expired");

            IsActive = true;
            ActivatedOn = DateTime.UtcNow;
            ActivationToken = Guid.NewGuid().ToString();
            ActivationTokenExpiry = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("User is already deactivated");

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateProfile(string email, string name)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Email = email;
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
        public void AssignRole(Role role, string assignedBy)
        {
            if (Roles.Any(r => r.RoleName == role))
                return;

            var userRole = UserRole.Create(this.Id, role, assignedBy);
            Roles.Add(userRole);

            UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveRole(Role role)
        {
            var userRole = Roles.FirstOrDefault(r => r.RoleName == role);
            if (userRole != null)
            {
                Roles.Remove(userRole);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        public bool HasRole(Role role)
        {
            return Roles.Any(r => r.RoleName == role);
        }
        public bool HasAnyRole(params Role[] roles)
        {
            return Roles.Any(r => roles.Contains(r.RoleName));
        }
        public IEnumerable<Role> GetRoles()
        {
            return Roles.Select(r => r.RoleName);
        }
        public IEnumerable<string> GetRoleNames()
        {
            return Roles.Select(r => r.RoleName.ToString());
        }
        public bool IsAdministrator()
        {
            return HasRole(Role.Administrator);
        }
        public bool CanPerformAction(params Role[] requiredRoles)
        {
            return IsActive && HasAnyRole(requiredRoles);
        }
        public bool IsActivationTokenValid()
        {
            return !string.IsNullOrEmpty(ActivationToken)
                   && ActivationTokenExpiry.HasValue
                   && ActivationTokenExpiry.Value > DateTime.UtcNow;
        }
        private static string GenerateSecureToken()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        public void RegenerateActivationToken(TimeSpan validFor)
        {
            if (IsActive)
                throw new InvalidOperationException("Cannot regenerate token for already active user");

            ActivationToken = Guid.NewGuid().ToString();
            ActivationTokenExpiry = DateTime.UtcNow.Add(validFor);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
