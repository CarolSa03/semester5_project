using System.ComponentModel.DataAnnotations;

namespace PortManagement.Application.DTOs;

public class CreateVesselVisitNotificationDto
{
    [Required(ErrorMessage = "Business ID is required")]
    [RegularExpression(@"^\d{4}-[A-Z0-9]{2,10}-\d{6}$",
        ErrorMessage = "Business ID format: YYYY-PORTCODE-NNNNNN (e.g., 2026-LEIXOES-000001)")]
    public required string BusinessId { get; set; }

    [Required(ErrorMessage = "Vessel ID is required")]
    public required Guid VesselId { get; set; }

    [Required(ErrorMessage = "Shipping agent representative ID is required")]
    public required int ShippingAgentRepresentativeId { get; set; }

    [Required(ErrorMessage = "ETA is required")]
    public required DateTime ETA { get; set; }

    [Required(ErrorMessage = "ETD is required")]
    public required DateTime ETD { get; set; }
}
