using System.ComponentModel.DataAnnotations;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Application.DTOs;

public class VesselVisitNotificationDto
{
    public Guid Id { get; set; }
    public string BusinessId { get; set; } = string.Empty;
    public Guid? VesselId { get; set; }
    public int? ShippingAgentRepresentativeId { get; set; }
    public DateTime? ETA { get; set; }
    public DateTime? ETD { get; set; }
    public List<Guid>? CargoManifestsId { get; set; } = new();
    public Guid? CrewId { get; set; }
    public string? Status { get; set; }
    public Guid? ApprovedByOfficerId { get; set; }
    public Guid? RejectedByOfficerId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? ApprovalNotes { get; set; }
    public Guid? AssignedDockId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static explicit operator VesselVisitNotificationDto(VesselVisitNotification entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new VesselVisitNotificationDto
        {
            Id = entity.Id,
            BusinessId = entity.BusinessId.Value,
            VesselId = entity.Vessel?.Id,
            ShippingAgentRepresentativeId = entity.ShippingAgentRepresentative?.Id,
            ETA = entity.ETA,
            ETD = entity.ETD,
            CargoManifestsId = entity.CargoManifests?.Select(cm => cm.Id).ToList() ?? new List<Guid>(),
            CrewId = entity.Crew?.Id,
            Status = entity.Status?.ToString(),
            ApprovedByOfficerId = entity.ApprovedByOfficerId,
            RejectedByOfficerId = entity.RejectedByOfficerId,
            ApprovedAt = entity.ApprovedAt,
            RejectedAt = entity.RejectedAt,
            ApprovalNotes = entity.ApprovalNotes,
            AssignedDockId = entity.AssignedDock?.Id,
            RejectionReason = entity.RejectionReason,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

public class ApproveVesselVisitDto
{
    [Required(ErrorMessage = "Dock ID is required")]
    public required Guid DockId { get; set; }

    [StringLength(500, ErrorMessage = "Approval notes cannot exceed 500 characters")]
    public string? ApprovalNotes { get; set; }
}

public class RejectVesselVisitDto
{
    [Required(ErrorMessage = "Rejection reason is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Rejection reason must be between 10 and 500 characters")]
    public required string RejectionReason { get; set; }
}

public class WithdrawVesselVisitDto
{
    [StringLength(500, ErrorMessage = "Withdrawal reason cannot exceed 500 characters")]
    public string? WithdrawalReason { get; set; }
}
