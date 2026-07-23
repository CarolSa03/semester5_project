using PortManagement.Domain.Staff.Entities;
using PortManagement.Domain.Staff.ValueObjects;

namespace PortManagement.Application.DTOs;

public class StaffMemberDTO
{
    public Guid? Id { get; set; }
    
    public string StaffMemberId { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty; 
    
    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public List<Guid> Qualifications { get; set; } = new();

    public bool IsAvailable { get; set; }

    public static explicit operator StaffMember(StaffMemberDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        
        var id = dto.Id ?? Guid.NewGuid();
        
        var staffMemberId = new SMId(dto.StaffMemberId);
        var shortName = new SMName(dto.ShortName);
        var email = new Email(dto.Email);
        var phoneNumber = new PhoneNumber(dto.PhoneNumber);
        var operationalWindow = new SMOperationalWindow("24/7");
        
        var staffMember = new StaffMember(
            staffMemberId,
            shortName,
            email,
            phoneNumber,
            operationalWindow,
            dto.Qualifications,
            dto.IsAvailable
        );
        
        staffMember.Id = id;
        
        return staffMember;
    }
    
    public static explicit operator StaffMemberDTO(StaffMember entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        return new StaffMemberDTO
        {
            Id =  entity.Id,
            StaffMemberId = entity.StaffMemberId.Value,
            ShortName = entity.ShortName.Value,
            Email = entity.Email.Value,
            PhoneNumber = entity.PhoneNumber.Value,
            Qualifications = entity.QualificationIds.ToList(),
            IsAvailable = entity.IsAvailable
        };
    }
}
