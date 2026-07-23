using System;

namespace PortManagement.Domain.Auth
{
    public class UserRole
    {
        public Guid Id { get; private set; }
        public Role RoleName { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime AssignedAt { get; private set; }
        public string AssignedBy { get; private set; }

        public virtual AppUser User { get; private set; }

        private UserRole()
        {
        }

        public static UserRole Create(Guid userId, Role roleName, string assignedBy)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(assignedBy))
                throw new ArgumentException("AssignedBy is required for audit purposes", nameof(assignedBy));

            if (!Enum.IsDefined(typeof(Role), roleName))
                throw new ArgumentException("Invalid role specified", nameof(roleName));

            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleName = roleName,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow
            };

            return userRole;
        }

        public bool IsRole(Role role)
        {
            return RoleName == role;
        }

        public bool IsAdministrativeRole()
        {
            return RoleName == Role.Administrator;
        }

        public string GetRoleDescription()
        {
            return RoleName switch
            {
                Role.Administrator => "System Administrator",
                Role.PortAuthorityOfficer => "Port Authority Officer",
                Role.ShippingAgentRepresentative => "Shipping Agent Representative",
                Role.LogisticsOperator => "Logistics Operator",
                _ => RoleName.ToString()
            };
        }
    }

    public enum Role
    {
        Administrator = 1,
        PortAuthorityOfficer = 2,
        ShippingAgentRepresentative = 3,
        LogisticsOperator = 4
    }

    public static class RoleExtensions
    {
        public static Role[] GetAllRoles()
        {
            return (Role[])Enum.GetValues(typeof(Role));
        }
        public static string ToRoleString(this Role role)
        {
            return role.ToString();
        }
        public static string ToRoleDescription(this Role role)
        {
            return role switch
            {
                Role.Administrator => "System Administrator",
                Role.PortAuthorityOfficer => "Port Authority Officer",
                Role.ShippingAgentRepresentative => "Shipping Agent Representative",
                Role.LogisticsOperator => "Logistics Operator",
                _ => role.ToString()
            };
        }
        public static bool TryParseRole(string roleString, out Role role)
        {
            return Enum.TryParse(roleString, true, out role) && Enum.IsDefined(typeof(Role), role);
        }
        public static bool IsValidRole(string roleString)
        {
            return Enum.TryParse<Role>(roleString, true, out var role) && Enum.IsDefined(typeof(Role), role);
        }
        public static bool IsAdministrative(this Role role)
        {
            return role == Role.Administrator;
        }
        public static Role[] GetManagementRoles()
        {
            return new[] { Role.Administrator };
        }
        public static Role[] GetVesselApprovalRoles()
        {
            return new[] { Role.PortAuthorityOfficer };
        }
        public static Role[] GetVesselSubmissionRoles()
        {
            return new[] { Role.ShippingAgentRepresentative };
        }
        public static Role[] GetSchedulingRoles()
        {
            return new[] { Role.LogisticsOperator };
        }
    }
}
