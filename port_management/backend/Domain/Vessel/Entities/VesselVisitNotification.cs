using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;

public class VesselVisitNotification
{
    public Guid Id { get; private set; }
    
    private string _businessId;
    
    public VesselVisitBusinessId BusinessId 
    { 
        get => string.IsNullOrEmpty(_businessId) ? null : VesselVisitBusinessId.Parse(_businessId);
        private set => _businessId = value?.Value;
    }
    
    public VesselRecord? Vessel { get; set; }
    public ShippingAgentRepresentative? ShippingAgentRepresentative { get; set; }
    public DateTime? ETA { get; set; }
    public DateTime? ETD { get; set; }
    public List<CargoManifest>? CargoManifests { get; set; }
    public CrewInfo? Crew { get; set; }
    public VesselNotificationStatus? Status { get; set; }
    public Guid? ApprovedByOfficerId { get; set; }
    public Guid? RejectedByOfficerId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? ApprovalNotes { get; set; }
    public Dock? AssignedDock { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Construtor vazio para EF Core
    private VesselVisitNotification() { }

    // Construtor público que recebe o BusinessId
    public VesselVisitNotification(VesselVisitBusinessId businessId)
    {
        if (businessId == null)
            throw new ArgumentNullException(nameof(businessId));

        Id = Guid.NewGuid();
        _businessId = businessId.Value;
        Status = VesselNotificationStatus.InProgress;
        CreatedAt = DateTime.UtcNow;
    }

    public void ValidateForSubmission()
    {
        if (Vessel == null)
            throw new InvalidOperationException("Vessel information is required");

        if (ShippingAgentRepresentative == null)
            throw new InvalidOperationException("Shipping agent representative is required");

        if (ETA >= ETD)
            throw new InvalidOperationException("ETD must be after ETA");

        if (ETA < DateTime.UtcNow)
            throw new InvalidOperationException("ETA cannot be in the past");

        if (Status != VesselNotificationStatus.InProgress)
            throw new InvalidOperationException("Only notifications in progress can be submitted");
    }

    public bool RequiresCrewInfoForSecurity()
    {
        return Vessel?.Imo != null;
    }
}