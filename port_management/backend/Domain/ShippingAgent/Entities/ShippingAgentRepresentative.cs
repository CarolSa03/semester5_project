namespace PortManagement.Domain.ShippingAgent.Entities
{
    public class ShippingAgentRepresentative
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ShippingAgentOrganizationId { get; set; } = string.Empty;
        public ShippingAgentOrganization ShippingAgentOrganization { get; set; } = null!;
    }
}
