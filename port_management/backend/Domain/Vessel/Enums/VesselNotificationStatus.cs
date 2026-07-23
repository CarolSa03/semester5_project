namespace PortManagement.Domain.Vessel.Enums;

public enum VesselNotificationStatus
{
    InProgress,      // Notification is being created/edited
    PendingApproval, // Submitted and waiting for port authority approval
    Approved,        // Approved by port authority
    Rejected,        // Rejected by port authority
    Withdrawn        // Withdrawn by shipping agent
}
