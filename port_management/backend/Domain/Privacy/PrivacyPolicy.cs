namespace PortManagement.Domain.Privacy
{
    public class PrivacyPolicy
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
