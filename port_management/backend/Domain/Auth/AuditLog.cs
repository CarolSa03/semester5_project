using System;

namespace PortManagement.Domain.Auth
{
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public AuditEventType EventType { get; private set; }
        public string Details { get; private set; }
        public string PerformedBy { get; private set; }
        public string? AffectedUser { get; private set; }
        public string? IpAddress { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int? StatusCode { get; private set; }
        public string? Metadata { get; private set; }
        private AuditLog()
        {
        }
        public static AuditLog Create(
            AuditEventType eventType,
            string performedBy,
            string details,
            string? ipAddress = null,
            string? affectedUser = null,
            int? statusCode = null,
            string? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(performedBy))
                throw new ArgumentException("PerformedBy is required for audit logging", nameof(performedBy));

            if (string.IsNullOrWhiteSpace(details))
                throw new ArgumentException("Details are required for audit logging", nameof(details));

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                PerformedBy = performedBy,
                Details = details,
                IpAddress = ipAddress,
                AffectedUser = affectedUser,
                StatusCode = statusCode,
                Metadata = metadata,
                Timestamp = DateTime.UtcNow
            };

            return auditLog;
        }
        public static AuditLog CreateAuthorizationSuccess(
            string performedBy,
            string resource,
            string action,
            string ipAddress = null)
        {
            var details = $"User successfully authorized to {action} on {resource}";

            return Create(
                eventType: AuditEventType.AuthorizationSuccess,
                performedBy: performedBy,
                details: details,
                ipAddress: ipAddress,
                statusCode: 200
            );
        }
        public static AuditLog CreateAuthorizationFailure(
            string performedBy,
            string resource,
            string action,
            string reason,
            string ipAddress = null)
        {
            var details = $"User unauthorized to {action} on {resource}. Reason: {reason}";

            return Create(
                eventType: AuditEventType.AuthorizationFailure,
                performedBy: performedBy,
                details: details,
                ipAddress: ipAddress,
                statusCode: 403
            );
        }
        public static AuditLog CreateUnauthorizedAccess(
            string performedBy,
            string attemptedResource,
            string ipAddress = null)
        {
            var details = $"Unauthorized access attempt to {attemptedResource}";

            return Create(
                eventType: AuditEventType.UnauthorizedAccess,
                performedBy: performedBy,
                details: details,
                ipAddress: ipAddress,
                statusCode: 401
            );
        }
        public static AuditLog CreateLoginEvent(
            bool isSuccess,
            string performedBy,
            string? ipAddress = null,
            string? failureReason = null)
        {
            var eventType = isSuccess ? AuditEventType.LoginSuccess : AuditEventType.LoginFailure;
            var details = isSuccess
                ? "User successfully authenticated"
                : $"Authentication failed. Reason: {failureReason ?? "Unknown"}";
            var statusCode = isSuccess ? 200 : 401;

            return Create(
                eventType: eventType,
                performedBy: performedBy,
                details: details,
                ipAddress: ipAddress,
                statusCode: statusCode
            );
        }
        public static AuditLog CreateUserManagementEvent(
            AuditEventType eventType,
            string performedBy,
            string affectedUser,
            string details,
            string ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(affectedUser))
                throw new ArgumentException("AffectedUser is required for user management events", nameof(affectedUser));

            return Create(
                eventType: eventType,
                performedBy: performedBy,
                details: details,
                ipAddress: ipAddress,
                affectedUser: affectedUser,
                statusCode: 200
            );
        }
        public static AuditLog CreateRoleManagementEvent(
            AuditEventType eventType,
            string performedBy,
            string affectedUser,
            string roleName,
            string? ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(affectedUser))
                throw new ArgumentException("AffectedUser is required for role management events", nameof(affectedUser));

            string action = eventType switch
            {
                AuditEventType.RoleAssigned => "assigned",
                AuditEventType.RoleRemoved => "removed",
                AuditEventType.RoleUpdated => "updated",
                _ => "modified"
            };

            var details = $"Role '{roleName}' {action} for user '{affectedUser}'";

            return Create(
                eventType: eventType,
                performedBy: performedBy,
                details: details,
                ipAddress: ipAddress,
                affectedUser: affectedUser,
                statusCode: 200
            );
        }
        public static AuditLog CreateSecurityViolation(
            string performedBy,
            string violationType,
            string details,
            string? ipAddress = null)
        {
            var fullDetails = $"Security violation detected: {violationType}. {details}";

            return Create(
                eventType: AuditEventType.SecurityViolation,
                performedBy: performedBy,
                details: fullDetails,
                ipAddress: ipAddress,
                statusCode: 403
            );
        }
        public bool IsFailureEvent()
        {
            return EventType is
                AuditEventType.LoginFailure or
                AuditEventType.AuthorizationFailure or
                AuditEventType.UnauthorizedAccess or
                AuditEventType.SecurityViolation or
                AuditEventType.SuspiciousActivity;
        }
        public bool IsSecurityCritical()
        {
            return EventType is
                AuditEventType.UnauthorizedAccess or
                AuditEventType.SecurityViolation or
                AuditEventType.SuspiciousActivity or
                AuditEventType.AuthorizationFailure;
        }
    }
    public enum AuditEventType
    {
        LoginSuccess,
        LoginFailure,
        LogoutSuccess,
        TokenRefresh,
        TokenExpired,
        AuthorizationSuccess,
        AuthorizationFailure,
        UnauthorizedAccess,
        UserCreated,
        UserActivated,
        UserDeactivated,
        UserUpdated,
        ActivationLinkSent,
        ActivationLinkExpired,
        RoleAssigned,
        RoleRemoved,
        RoleUpdated,
        SuspiciousActivity,
        SecurityViolation
    }
}
