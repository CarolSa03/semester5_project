using PortManagement.Application.DTOs;
using System.Text.RegularExpressions;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Staff.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services;


public class StaffMemberService : IStaffMemberService
{
    private readonly IStaffMemberRepository _repository;

    public StaffMemberService(IStaffMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<StaffMember?> GetStaffMemberByGuidAsync(Guid guid)
    {
        return await _repository.GetByGuIdAsync(guid);
    }

    public async Task CreateStaffMemberAsync(StaffMember staffMember)
    {
        ArgumentNullException.ThrowIfNull(staffMember);

        var existing = await _repository.GetByIdAsync(staffMember.StaffMemberId.Value);
        if (existing != null)
        {
            throw new InvalidOperationException($"A staff member with ID '{staffMember.StaffMemberId.Value}' already exists.");
        }

        await _repository.AddAsync(staffMember);
    }


    public async Task<StaffMember?> GetStaffMemberByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }


    public async Task<IEnumerable<StaffMember>> GetAvailableStaffMembersAsync()
    {
        var staffMembers = await _repository.GetByStatusAsync(true);


        return staffMembers;
    }


    public async Task<IEnumerable<StaffMember>> GetUnavailableStaffMembersAsync()
    {
        var staffMembers = await _repository.GetByStatusAsync(false);

        return staffMembers;
    }


    public async Task<IEnumerable<StaffMember>> GetStaffMembersByNameAsync(string name)
    {
        return await _repository.GetByNameAsync(name);
    }


    public async Task<IEnumerable<StaffMember>> GetStaffMembersByQualificationAsync(string qualification)
    {
        return await _repository.GetByQualificationAsync(qualification);
    }


    public async Task<IEnumerable<StaffMember>> GetAllStaffMembersAsync()
    {
        return await _repository.GetAllAsync();
    }


    public async Task UpdateStaffMemberAsync(StaffMember staffMember)
    {
        ArgumentNullException.ThrowIfNull(staffMember);

        var existing = await _repository.GetByIdAsync(staffMember.StaffMemberId.Value);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Staff member with ID '{staffMember.StaffMemberId.Value}' not found.");
        }

        await _repository.UpdateAsync(staffMember);

    }


    public async Task DeactivateStaffMemberAsync(Guid id)
    {
        var existing = await _repository.GetByGuidAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Staff member with ID '{id}' not found.");
        }

        await _repository.DeactivateAsync(id);
    }

    public async Task ActivateStaffMemberAsync(Guid id)
    {
        var existing = await _repository.GetByGuidAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Staff member with ID '{id}' not found.");
        }

        await _repository.ActivateAsync(id);
    }

    public async Task RemoveStaffMemberAsync(Guid id)
    {
        await _repository.RemoveAsync(id);
    }

}
