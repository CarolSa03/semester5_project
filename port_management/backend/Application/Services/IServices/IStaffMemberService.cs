using PortManagement.Application.DTOs;
using PortManagement.Domain.Staff.Entities;

namespace PortManagement.Application.Services.IServices;

public interface IStaffMemberService
{
    Task CreateStaffMemberAsync(StaffMember staffMember);
    Task<StaffMember?> GetStaffMemberByIdAsync(string id);

    Task<StaffMember?> GetStaffMemberByGuidAsync(Guid id);
    Task<IEnumerable<StaffMember>> GetAvailableStaffMembersAsync();
    Task<IEnumerable<StaffMember>> GetUnavailableStaffMembersAsync();
    Task<IEnumerable<StaffMember>> GetStaffMembersByNameAsync(string name);
    Task<IEnumerable<StaffMember>> GetStaffMembersByQualificationAsync(string qualification);
    Task<IEnumerable<StaffMember>> GetAllStaffMembersAsync();
    Task UpdateStaffMemberAsync(StaffMember staffMember);
    Task DeactivateStaffMemberAsync(Guid id);
    Task ActivateStaffMemberAsync(Guid id);
    Task RemoveStaffMemberAsync(Guid id);
}
