using Microsoft.EntityFrameworkCore;
using PortManagement.Application.Common.Interfaces;
using PortManagement.Domain.Auth;
using PortManagement.Infrastructure.Data;

namespace PortManagement.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly PortManagementDbContext _context;

        public AuditLogRepository(PortManagementDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
            return auditLog;
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAsync(string performedBy, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(log => log.PerformedBy == performedBy)
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByEventTypeAsync(AuditEventType eventType, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(log => log.EventType == eventType)
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetSecurityCriticalEventsAsync(int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(log => log.EventType == AuditEventType.UnauthorizedAccess ||
                             log.EventType == AuditEventType.SecurityViolation ||
                             log.EventType == AuditEventType.SuspiciousActivity ||
                             log.EventType == AuditEventType.AuthorizationFailure)
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetFailureEventsAsync(int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(log => log.EventType == AuditEventType.LoginFailure ||
                             log.EventType == AuditEventType.AuthorizationFailure ||
                             log.EventType == AuditEventType.UnauthorizedAccess ||
                             log.EventType == AuditEventType.SecurityViolation ||
                             log.EventType == AuditEventType.SuspiciousActivity)
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(log => log.Timestamp >= startDate && log.Timestamp <= endDate)
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
