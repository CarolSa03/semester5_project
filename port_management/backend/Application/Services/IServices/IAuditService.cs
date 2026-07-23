using PortManagement.Application.DTOs.Auth;
using PortManagement.Domain.Auth;

namespace PortManagement.Application.Services.IServices
{
    public interface IAuditService
    {
        Task LogAuthorizationSuccessAsync(string performedBy, string resource, string action, string ipAddress = null);
        Task LogAuthorizationFailureAsync(string performedBy, string resource, string action, string reason, string ipAddress = null);
        Task LogUnauthorizedAccessAsync(string performedBy, string attemptedResource, string ipAddress = null);
        Task LogLoginEventAsync(bool isSuccess, string performedBy, string ipAddress = null, string failureReason = null);
        Task LogLogoutEventAsync(string performedBy, string ipAddress = null);
        Task LogUserManagementEventAsync(AuditEventType eventType, string performedBy, string affectedUser, string details, string ipAddress = null);
        Task LogRoleManagementEventAsync(AuditEventType eventType, string performedBy, string affectedUser, string roleName, string ipAddress = null);
        Task LogSecurityViolationAsync(string performedBy, string violationType, string details, string ipAddress = null);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsByUserAsync(string performedBy, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLogDto>> GetSecurityCriticalEventsAsync(int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLogDto>> GetFailureEventsAsync(int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50);
    }
}