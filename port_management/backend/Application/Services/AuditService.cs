using PortManagement.Application.Common.Interfaces;
using PortManagement.Application.DTOs.Auth;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Auth;

namespace PortManagement.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        //3.2.4        
        public async Task LogAuthorizationSuccessAsync(string performedBy, string resource, string action, string ipAddress = null)
        {
            var auditLog = AuditLog.CreateAuthorizationSuccess(performedBy, resource, action, ipAddress);
            await _auditLogRepository.AddAsync(auditLog);
        }

        //3.2.4
        public async Task LogAuthorizationFailureAsync(string performedBy, string resource, string action, string reason, string ipAddress = null)
        {
            var auditLog = AuditLog.CreateAuthorizationFailure(performedBy, resource, action, reason, ipAddress);
            await _auditLogRepository.AddAsync(auditLog);
        }

        //3.2.4
        public async Task LogUnauthorizedAccessAsync(string performedBy, string attemptedResource, string ipAddress = null)
        {
            var auditLog = AuditLog.CreateUnauthorizedAccess(performedBy, attemptedResource, ipAddress);
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task LogLoginEventAsync(bool isSuccess, string performedBy, string ipAddress = null, string failureReason = null)
        {
            var auditLog = AuditLog.CreateLoginEvent(isSuccess, performedBy, ipAddress, failureReason);
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task LogLogoutEventAsync(string performedBy, string ipAddress = null)
        {
            var auditLog = AuditLog.Create(
                AuditEventType.LogoutSuccess,
                performedBy,
                "User logged out successfully",
                ipAddress,
                statusCode: 200
            );
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task LogUserManagementEventAsync(AuditEventType eventType, string performedBy, string affectedUser, string details, string ipAddress = null)
        {
            var auditLog = AuditLog.CreateUserManagementEvent(eventType, performedBy, affectedUser, details, ipAddress);
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task LogRoleManagementEventAsync(AuditEventType eventType, string performedBy, string affectedUser, string roleName, string ipAddress = null)
        {
            var auditLog = AuditLog.CreateRoleManagementEvent(eventType, performedBy, affectedUser, roleName, ipAddress);
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task LogSecurityViolationAsync(string performedBy, string violationType, string details, string ipAddress = null)
        {
            var auditLog = AuditLog.CreateSecurityViolation(performedBy, violationType, details, ipAddress);
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByUserAsync(string performedBy, int pageNumber = 1, int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetByUserAsync(performedBy, pageNumber, pageSize);
            return AuthMapper.ToDtoList(logs);
        }

        public async Task<IEnumerable<AuditLogDto>> GetSecurityCriticalEventsAsync(int pageNumber = 1, int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetSecurityCriticalEventsAsync(pageNumber, pageSize);
            return AuthMapper.ToDtoList(logs);
        }

        public async Task<IEnumerable<AuditLogDto>> GetFailureEventsAsync(int pageNumber = 1, int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetFailureEventsAsync(pageNumber, pageSize);
            return AuthMapper.ToDtoList(logs);
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
            return AuthMapper.ToDtoList(logs);
        }
    }
}