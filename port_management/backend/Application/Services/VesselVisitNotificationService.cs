using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services
{
    public class DomainValidationException : Exception
    {
        public DomainValidationException(string message) : base(message) { }
    }

    public class BusinessRuleViolationException : Exception
    {
        public BusinessRuleViolationException(string message) : base(message) { }
    }

    public class VesselVisitNotificationService : IVesselVisitNotificationService
    {
        private readonly IVesselVisitNotificationRepository _repository;
        private readonly IShippingAgentRepresentativeRepository _representativeRepository;
        private readonly IVesselRecordRepository _vesselRecordRepository;
        private readonly IDockRepository _dockRepository;
        private readonly string _portCode;

        public VesselVisitNotificationService(
            IVesselVisitNotificationRepository repository,
            IShippingAgentRepresentativeRepository representativeRepository,
            IVesselRecordRepository vesselRecordRepository,
            IDockRepository dockRepository,
            string portCode)
        {
            _repository = repository;
            _representativeRepository = representativeRepository;
            _vesselRecordRepository = vesselRecordRepository;
            _dockRepository = dockRepository;
            _portCode = portCode;
        }

        public async Task<List<VesselVisitNotificationDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await _repository.GetAllAsync(ct);
            return entities.Select(VesselVisitNotificationMapper.ToDto).ToList();
        }

        public async Task<VesselVisitNotificationDto> CreateAsync(VesselVisitNotificationDto dto, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.BusinessId))
                    throw new DomainValidationException("Business ID is required");

                if (!dto.VesselId.HasValue)
                    throw new DomainValidationException("Vessel ID is required");

                if (!dto.ShippingAgentRepresentativeId.HasValue)
                    throw new DomainValidationException("Shipping agent representative ID is required");

                if (!dto.ETA.HasValue || !dto.ETD.HasValue)
                    throw new DomainValidationException("ETA and ETD are required");

                if (dto.ETD <= dto.ETA)
                    throw new DomainValidationException("ETD must be after ETA");

                VesselVisitBusinessId businessId = VesselVisitBusinessId.Parse(dto.BusinessId);

                var existing = await _repository.GetByBusinessIdAsync(businessId, ct);
                if (existing != null)
                    throw new DomainValidationException($"Business ID '{dto.BusinessId}' already exists");

                var entity = new VesselVisitNotification(businessId)
                {
                    ETA = dto.ETA.Value,
                    ETD = dto.ETD.Value
                };

                var vesselRecord = await _vesselRecordRepository.GetByIdAsync(dto.VesselId.Value, ct);
                if (vesselRecord == null)
                    throw new DomainValidationException($"Vessel with ID {dto.VesselId} not found");
                entity.Vessel = vesselRecord;

                var representative = await _representativeRepository.GetByIdAsync(
                    dto.ShippingAgentRepresentativeId.Value, ct);
                if (representative == null)
                    throw new DomainValidationException(
                        $"Representative with ID {dto.ShippingAgentRepresentativeId} not found");
                entity.ShippingAgentRepresentative = representative;

                var createdEntity = await _repository.AddAsync(entity, ct);
                return VesselVisitNotificationMapper.ToDto(createdEntity);
            }
            catch (Exception ex) when (ex is not DomainValidationException and not BusinessRuleViolationException)
            {
                throw new BusinessRuleViolationException($"Failed to create notification: {ex.Message}");
            }
        }

        public async Task SubmitAsync(string businessId, CancellationToken ct = default)
        {
            var id = VesselVisitBusinessId.Parse(businessId);
            var entity = await _repository.GetByBusinessIdAsync(id, ct);

            if (entity == null)
                throw new DomainValidationException("Notification not found");

            try
            {
                entity.ValidateForSubmission();

                // Check crew requirements for security compliance
                if (entity.RequiresCrewInfoForSecurity() && (entity.Crew == null || !entity.Crew.IsCompliant()))
                {
                    throw new BusinessRuleViolationException("Crew information is required and must be complete for security compliance");
                }

                entity.Status = VesselNotificationStatus.PendingApproval;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity, ct);
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessRuleViolationException(ex.Message);
            }
        }

        public async Task<VesselVisitNotificationDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repository.GetByIdAsync(id, ct);
            return entity != null ? VesselVisitNotificationMapper.ToDto(entity) : null;
        }

        public async Task UpdateAsync(string businessId, VesselVisitNotificationDto dto, CancellationToken ct = default)
        {
            var businessIdObj = VesselVisitBusinessId.Parse(businessId);
            var entity = await _repository.GetByBusinessIdAsync(businessIdObj, ct);

            if (entity == null)
                throw new DomainValidationException("Notification not found");

            if (entity.Status != VesselNotificationStatus.InProgress && entity.Status != VesselNotificationStatus.Rejected)
                throw new BusinessRuleViolationException("Only notifications in progress or rejected can be updated.");

            // Validate dates if both are provided
            var newETA = dto.ETA ?? entity.ETA;
            var newETD = dto.ETD ?? entity.ETD;
            if (newETD <= newETA)
            {
                throw new DomainValidationException("ETD must be after ETA");
            }

            // If rejected notification is being updated, change status back to InProgress
            if (entity.Status == VesselNotificationStatus.Rejected)
            {
                entity.Status = VesselNotificationStatus.InProgress;
                entity.RejectionReason = null;
                entity.RejectedByOfficerId = null;
                entity.RejectedAt = null;
            }

            // Update fields

            // If the incoming DTO explicitly provides a VesselImo, update the entity's Vessel reference accordingly
            if (dto.VesselId != null)
            {
                if (string.IsNullOrWhiteSpace(dto.VesselId.ToString()))
                {
                    // Explicitly clearing the vessel reference
                    entity.Vessel = null;
                }
                else
                {
                    // Note: repository method name uses GetByIMOAsync; ensure correct casing
                    var vessel = await _vesselRecordRepository.GetByIdAsync(dto.Id, ct);
                    if (vessel == null)
                        throw new DomainValidationException($"Vessel with ID {dto.VesselId} not found");

                    entity.Vessel = vessel;
                }
            }

            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity, ct);
        }

        public async Task DeleteAsync(string businessId, CancellationToken ct = default)
        {
            var businessIdObj = VesselVisitBusinessId.Parse(businessId);
            var entity = await _repository.GetByBusinessIdAsync(businessIdObj, ct);

            if (entity == null)
                throw new DomainValidationException("Notification not found");

            if (entity.Status != VesselNotificationStatus.InProgress)
                throw new BusinessRuleViolationException("Only notifications in progress can be deleted.");

            await _repository.DeleteAsync(entity, ct);
        }

        public async Task WithdrawAsync(string businessId, string? withdrawalReason = null, CancellationToken ct = default)
        {
            var businessIdObj = VesselVisitBusinessId.Parse(businessId);
            var entity = await _repository.GetByBusinessIdAsync(businessIdObj, ct);

            if (entity == null)
                throw new DomainValidationException("Notification not found");

            if (entity.Status != VesselNotificationStatus.InProgress && entity.Status != VesselNotificationStatus.PendingApproval)
                throw new BusinessRuleViolationException("Only notifications in progress or pending approval can be withdrawn.");

            entity.Status = VesselNotificationStatus.Withdrawn;
            entity.RejectionReason = string.IsNullOrWhiteSpace(withdrawalReason)
                ? "Withdrawn by agent"
                : $"Withdrawn by agent: {withdrawalReason}";
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity, ct);
        }

        public async Task<VesselVisitNotificationDto?> GetByBusinessIdAsync(string businessId, CancellationToken ct = default)
        {
            var businessIdObj = VesselVisitBusinessId.Parse(businessId);
            var entity = await _repository.GetByBusinessIdAsync(businessIdObj, ct);
            return entity != null ? VesselVisitNotificationMapper.ToDto(entity) : null;
        }

        public async Task<IEnumerable<VesselVisitNotificationDto>> GetByShippingAgentAsync(int agentId, CancellationToken ct = default)
        {
            var entities = await _repository.GetByShippingAgentAsync(agentId, ct);
            return entities.Select(VesselVisitNotificationMapper.ToDto);
        }

        public async Task<IEnumerable<VesselVisitNotificationDto>> GetByStatusAsync(VesselNotificationStatus status, CancellationToken ct = default)
        {
            var entities = await _repository.GetByStatusAsync(status, ct);
            return entities.Select(VesselVisitNotificationMapper.ToDto);
        }

        public async Task<List<VesselVisitNotificationDto>> GetForAgentAsync(
            VesselVisitNotificationDto dto,
            CancellationToken ct = default)
        {
            try
            {
                if (!dto.ShippingAgentRepresentativeId.HasValue)
                    throw new DomainValidationException("ShippingAgentRepresentativeId is required");

                var representative = await _representativeRepository.GetByIdAsync(dto.ShippingAgentRepresentativeId.Value, ct);
                if (representative == null)
                    throw new DomainValidationException("Representative not found");

                var repId = representative.Id;

                // Parse ID if provided
                Guid id = Guid.Empty;
                if (!string.IsNullOrWhiteSpace(dto.VesselId.ToString()))
                {
                    var vessel = await _vesselRecordRepository.GetByIdAsync(dto.Id, ct);
                    if (vessel == null)
                        throw new DomainValidationException($"Vessel with ID {dto.VesselId} not found");

                    id = vessel.Id;
                }

                // Parse status if provided (single status stored as string in DTO)
                VesselNotificationStatus? status = null;
                if (!string.IsNullOrWhiteSpace(dto.Status))
                {
                    if (Enum.TryParse<VesselNotificationStatus>(dto.Status, true, out var s))
                        status = s;
                    else
                        throw new DomainValidationException($"Invalid status: {dto.Status}");
                }

                // Use ETA/ETD as optional search window
                DateTime? start = dto.ETA;
                DateTime? end = dto.ETD;

                // Call repository (adapted to expected signature: repId, optional status, optional imo, optional start/end)
                var results = await _repository.SearchForAgentAsync(repId, status, id, start, end, ct);

                var distinctEntities = (results ?? new List<VesselVisitNotification>())
                    .GroupBy(e => e.Id)
                    .Select(g => g.First())
                    .ToList();

                return distinctEntities.Select(VesselVisitNotificationMapper.ToDto).ToList();
            }
            catch (Exception ex) when (ex is not DomainValidationException && ex is not BusinessRuleViolationException)
            {
                throw new BusinessRuleViolationException($"Search failed: {ex.Message}");
            }
        }



        public async Task ApproveAsync(string businessId, Guid dockId, Guid officerId, string? approvalNotes = null, CancellationToken ct = default)
        {
            var businessIdObj = VesselVisitBusinessId.Parse(businessId);
            var entity = await _repository.GetByBusinessIdAsync(businessIdObj, ct);

            if (entity == null)
                throw new DomainValidationException("Notification not found");

            if (entity.Status != VesselNotificationStatus.PendingApproval)
                throw new BusinessRuleViolationException("Only pending notifications can be approved");

            // Get and validate dock
            var dock = await _dockRepository.GetByIdAsync(dockId, ct);
            if (dock == null)
                throw new BusinessRuleViolationException($"Dock with ID {dockId} does not exist");

            // Check for dock availability
            var conflicts = await _repository.GetByDockAndTimeRangeAsync(
                dockId, (DateTime)entity.ETA, (DateTime)entity.ETD, ct);

            if (conflicts.Any(c => c.Id != entity.Id &&
                c.Status != VesselNotificationStatus.Rejected &&
                c.Status != VesselNotificationStatus.Withdrawn))
            {
                throw new BusinessRuleViolationException($"Dock '{dock.Name}' is already booked for the requested time period");
            }

            // Update entity
            entity.Status = VesselNotificationStatus.Approved;
            entity.AssignedDock = dock;
            entity.ApprovedByOfficerId = officerId;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.ApprovalNotes = approvalNotes;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity, ct);
        }

        public async Task RejectAsync(string businessId, string rejectionReason, Guid officerId, CancellationToken ct = default)
        {
            var businessIdObj = VesselVisitBusinessId.Parse(businessId);
            var entity = await _repository.GetByBusinessIdAsync(businessIdObj, ct);

            if (entity == null)
                throw new DomainValidationException("Notification not found");

            if (entity.Status != VesselNotificationStatus.PendingApproval)
                throw new BusinessRuleViolationException("Only pending notifications can be rejected");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new DomainValidationException("Rejection reason is required");

            if (rejectionReason.Trim().Length < 10)
                throw new DomainValidationException("Rejection reason must be at least 10 characters");

            entity.Status = VesselNotificationStatus.Rejected;
            entity.RejectionReason = rejectionReason;
            entity.RejectedByOfficerId = officerId;
            entity.RejectedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity, ct);
        }

        public async Task<IEnumerable<VesselVisitNotificationDto>> GetPendingNotificationsAsync(CancellationToken ct = default)
        {
            var entities = await _repository.GetByStatusAsync(VesselNotificationStatus.PendingApproval, ct);
            return entities.Select(VesselVisitNotificationMapper.ToDto);
        }


    }
}
