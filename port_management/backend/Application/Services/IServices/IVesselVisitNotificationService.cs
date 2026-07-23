using PortManagement.Application.DTOs;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;

namespace PortManagement.Application.Services.IServices
{
    public interface IVesselVisitNotificationService
    {
        Task<VesselVisitNotificationDto> CreateAsync(VesselVisitNotificationDto dto, CancellationToken ct = default);
        Task SubmitAsync(string businessId, CancellationToken ct = default);
        Task UpdateAsync(string businessId, VesselVisitNotificationDto dto, CancellationToken ct = default);
        Task DeleteAsync(string businessId, CancellationToken ct = default);
        Task WithdrawAsync(string businessId, string? withdrawalReason = null, CancellationToken ct = default);
        Task<VesselVisitNotificationDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<VesselVisitNotificationDto?> GetByBusinessIdAsync(string businessId, CancellationToken ct = default);
        Task<IEnumerable<VesselVisitNotificationDto>> GetByShippingAgentAsync(int agentId, CancellationToken ct = default);
        Task<List<VesselVisitNotificationDto>> GetForAgentAsync(
            VesselVisitNotificationDto searchDto,
            CancellationToken ct = default);
        Task<IEnumerable<VesselVisitNotificationDto>> GetByStatusAsync(VesselNotificationStatus status, CancellationToken ct = default);
        Task ApproveAsync(string businessId, Guid dockId, Guid officerId, string? approvalNotes = null,
            CancellationToken ct = default);
        Task RejectAsync(string businessId, string rejectionReason, Guid officerId, CancellationToken ct = default);
        Task<IEnumerable<VesselVisitNotificationDto>> GetPendingNotificationsAsync(CancellationToken ct = default);
        Task<List<VesselVisitNotificationDto>> GetAllAsync(CancellationToken ct = default);
    }
}
