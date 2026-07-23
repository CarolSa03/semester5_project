using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Application.Common.Interfaces
{
    public interface IVesselVisitNotificationRepository
    {
        Task<VesselVisitNotification?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<VesselVisitNotification?> GetByBusinessIdAsync(VesselVisitBusinessId businessId, CancellationToken ct = default);
        Task<List<VesselVisitNotification>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<VesselVisitNotification>> GetByShippingAgentAsync(int agentId, CancellationToken ct = default);
        Task<IEnumerable<VesselVisitNotification>> GetByStatusAsync(VesselNotificationStatus status, CancellationToken ct = default);
        Task<VesselVisitNotification> AddAsync(VesselVisitNotification notification, CancellationToken ct = default);
        Task UpdateAsync(VesselVisitNotification notification, CancellationToken ct = default);
        Task DeleteAsync(VesselVisitNotification notification, CancellationToken ct = default);
        Task<IEnumerable<VesselVisitNotification>> GetByDockAndTimeRangeAsync(Guid dockId,
            DateTime startTime,
            DateTime endTime,
            CancellationToken ct = default);
        Task<IReadOnlyList<VesselVisitNotification>> SearchForAgentAsync(int representativeId,
            VesselNotificationStatus? statuses,
            Guid? vesselId,
            DateTime? searchPeriodStart,
            DateTime? searchPeriodEnd,
            CancellationToken ct = default);
        Task<int> GetNextSequentialNumberAsync(int year, string portCode, CancellationToken ct = default);
        Task<bool> BusinessIdExistsAsync(VesselVisitBusinessId businessId, CancellationToken ct = default);
    }
}
