using PortManagement.Domain.Auth;

namespace PortManagement.Application.DTOs.Auth
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string Details { get; set; }
        public string PerformedBy { get; set; }
        public string AffectedUser { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public int? StatusCode { get; set; }
    }

    public class CreateAuditLogDto
    {
        public AuditEventType EventType { get; set; }
        public string PerformedBy { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }
        public string AffectedUser { get; set; }
        public int? StatusCode { get; set; }
    }
}