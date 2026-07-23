using PortManagement.Domain.Staff.Entities;

namespace PortManagement.Application.Common.Interfaces
{
    public interface IStaffMemberRepository
    {
        Task AddAsync(StaffMember staffMember);

        Task<StaffMember?> GetByGuIdAsync(Guid id);
        Task<StaffMember?> GetByIdAsync(string staffId);
        Task<IEnumerable<StaffMember>> GetAllAsync();
        Task UpdateAsync(StaffMember staffMember);
        Task<IEnumerable<StaffMember>> GetByStatusAsync(bool isAvailable);
        Task<IEnumerable<StaffMember>> GetByNameAsync(string name);
        Task<IEnumerable<StaffMember>> GetByQualificationAsync(string qualification);

        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
        Task<StaffMember?> GetByGuidAsync(Guid id);
        Task RemoveAsync(Guid id);
    }
}
