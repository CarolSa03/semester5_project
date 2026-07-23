using PortManagement.Domain.Auth;

namespace PortManagement.Application.Common.Interfaces {
    public interface IAuditLogRepository
    {
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetByUserAsync(string performedBy, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetByEventTypeAsync(AuditEventType eventType, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetSecurityCriticalEventsAsync(int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetFailureEventsAsync(int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50);
    }
}