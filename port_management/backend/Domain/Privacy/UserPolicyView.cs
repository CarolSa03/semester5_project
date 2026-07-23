namespace PortManagement.Domain.Privacy
{
    public class UserPolicyView
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public int PolicyVersion { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
