using PortManagement.Application.DTOs;
using PortManagement.Domain.Staff.Entities;

namespace PortManagement.Application.Mappers;

public class StaffMemberMapper
{
    public static StaffMemberDTO ToDto(StaffMember entity)
    {
        if(entity == null)
            throw new ArgumentNullException(nameof(entity));
        return entity switch
        {
            StaffMember staffMember => (StaffMemberDTO)staffMember,
            _ => throw new ArgumentException("Unknown entity type", nameof(entity))
        };
    }

    public static StaffMember ToEntity(StaffMemberDTO dto)
    {
        if(dto == null)
            throw new ArgumentNullException(nameof(dto));

        return dto switch
        {
            StaffMemberDTO staffMemberDto => (StaffMember)staffMemberDto,
            _ => throw new ArgumentException("Unknown DTO type", nameof(dto))
        };
    }
}
