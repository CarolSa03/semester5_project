using System.ComponentModel.DataAnnotations;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Application.DTOs
{
    // Data transfer object for crew information
    public class CrewInfoDto
    {
        public Guid Id { get; set; }
        // Name of the vessel captain
        [Required(ErrorMessage = "Captain name is required")]
        [StringLength(100, ErrorMessage = "Captain name cannot exceed 100 characters")]
        public string? CaptainName { get; set; }

        // Total number of crew members on board
        [Required(ErrorMessage = "Crew count is required")]
        [Range(1, 1000, ErrorMessage = "Crew count must be between 1 and 1000")]
        public int CrewCount { get; set; }

        // List of safety officers (required for dangerous cargo operations)
        [Required(ErrorMessage = "Safety officers list is required")]
        [MinLength(1, ErrorMessage = "At least one safety officer is required")]
        public List<CrewMemberDto> SafetyOfficers { get; set; } = new();

        public static explicit operator CrewInfo(CrewInfoDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new CrewInfo
            {
                CaptainName = new CICaptainName(dto.CaptainName),
                CrewCount = new CICrewCount(dto.CrewCount),
                SafetyOfficers = dto.SafetyOfficers?.Select(m => (CrewMember)m).ToList() ?? new()
            };
        }

        public static explicit operator CrewInfoDto(CrewInfo entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new CrewInfoDto
            {
                CaptainName = entity.CaptainName.Value,
                CrewCount = entity.CrewCount.Value,
                SafetyOfficers = entity.SafetyOfficers?.Select(m => (CrewMemberDto)m).ToList() ?? new()
            };
        }

    }

    // Data transfer object for individual crew member information
    public class CrewMemberDto
    {
        public Guid Id { get; set; }

        // Full name of the crew member
        [Required(ErrorMessage = "Crew member name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }

        // Government-issued identification number
        [Required(ErrorMessage = "Citizen ID is required")]
        [StringLength(50, ErrorMessage = "Citizen ID cannot exceed 50 characters")]
        public string? CitizenId { get; set; }

        // Country of citizenship
        [Required(ErrorMessage = "Nationality is required")]
        [StringLength(50, ErrorMessage = "Nationality cannot exceed 50 characters")]
        public string? Nationality { get; set; }

        public static explicit operator CrewMember(CrewMemberDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new CrewMember
            {
                Name = new CICrewMemberName(dto.Name),
                CitizenId = new CICitizenId(dto.CitizenId),
                Nationality = new CINationality(dto.Nationality)
            };
        }

        public static explicit operator CrewMemberDto(CrewMember entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new CrewMemberDto
            {
                Name = entity.Name.Value,
                CitizenId = entity.CitizenId.Value,
                Nationality = entity.Nationality.Value
            };
        }

    }
}
