using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Domain.Vessel.Entities
{
    // Represents information about the vessel's crew
    public class CrewInfo
    {
        public Guid Id { get; set; }

        public CICaptainName? CaptainName { get; set; } // Name of vessel captain
        public CICrewCount CrewCount { get; set; } = new CICrewCount(1); // Total number of crew members
        public List<CrewMember>? SafetyOfficers { get; set; } // Safety officers for dangerous cargo operations

        // Checks if crew information meets security compliance requirements
        public bool IsCompliant()
        {
            return CaptainName != null && !string.IsNullOrEmpty(CaptainName.Value) && // Captain name is required
                   CrewCount.Value > 0 && // Must have at least one crew member
                   SafetyOfficers != null && // Safety officers list must exist
                   SafetyOfficers.Count >= 1 && // At least one safety officer required for dangerous cargo
                   SafetyOfficers.All(o => // All safety officers must have complete information
                       o != null &&
                       o.Name != null && !string.IsNullOrEmpty(o.Name.Value) &&
                       o.CitizenId != null && !string.IsNullOrEmpty(o.CitizenId.Value) &&
                       o.Nationality != null && !string.IsNullOrEmpty(o.Nationality.Value));
        }

        // Validates the crew information, throwing exceptions if any required fields are missing or invalid
        public void Validate()
        {
            if (CaptainName == null || string.IsNullOrEmpty(CaptainName.Value))
                throw new InvalidOperationException("Captain name is required");

            if (CrewCount == null || CrewCount.Value <= 0)
                throw new InvalidOperationException("Crew count must be greater than 0");

            if (SafetyOfficers == null || SafetyOfficers.Count == 0)
                throw new InvalidOperationException("At least one safety officer is required");

            foreach (var officer in SafetyOfficers)
            {
                officer.Validate();
            }
        }
    }

    public class CrewMember
    {
        public Guid Id { get; set; }

        public CICrewMemberName? Name { get; set; } // Full name of crew member
        public CICitizenId? CitizenId { get; set; } // Government-issued identification number
        public CINationality? Nationality { get; set; } // Country of citizenship

        // Validates the crew member's information, ensuring all required fields are filled
        public void Validate()
        {
            if (Name == null || string.IsNullOrEmpty(Name.Value))
                throw new InvalidOperationException("Crew member name is required");

            if (CitizenId == null || string.IsNullOrEmpty(CitizenId.Value))
                throw new InvalidOperationException("Crew member citizen ID is required");

            if (Nationality == null || string.IsNullOrEmpty(Nationality.Value))
                throw new InvalidOperationException("Crew member nationality is required");
        }
    }
}
