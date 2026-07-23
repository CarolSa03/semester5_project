namespace PortManagement.Domain.ShippingAgent.Entities
{
    public class ShippingAgentOrganization
    {
        public string Id { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string AlternativeName { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<ShippingAgentRepresentative> Representatives { get; set; } = new List<ShippingAgentRepresentative>();
    }
}
