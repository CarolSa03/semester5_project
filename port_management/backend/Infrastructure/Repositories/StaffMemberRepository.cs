using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Staff.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;


namespace PortManagement.Infrastructure.Repositories;

public class StaffMemberRepository : IStaffMemberRepository
{
    private readonly PortManagementDbContext _context;

    public StaffMemberRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StaffMember staffMember)
    {
        await _context.StaffMembers.AddAsync(staffMember);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StaffMember staffMember)
    {
        _context.StaffMembers.Update(staffMember);
        await _context.SaveChangesAsync();
    }

    public async Task<StaffMember?> GetByGuIdAsync(Guid id)
    {
        return await _context.StaffMembers
            .FirstOrDefaultAsync(p => p.Id == id);
    }


    public async Task<StaffMember?> GetByIdAsync(string id)
    {
        return await _context.StaffMembers
            .FirstOrDefaultAsync(s => s.StaffMemberId.Value == id);
    }

    public async Task<IEnumerable<StaffMember>> GetAllAsync()
    {
        return await _context.StaffMembers.ToListAsync();
    }

    public async Task<IEnumerable<StaffMember>> GetByStatusAsync(bool isAvailable)
    {
        return await _context.StaffMembers
            .Where(s => s.IsAvailable == isAvailable)
            .ToListAsync();
    }


    public async Task<IEnumerable<StaffMember>> GetByNameAsync(string name)
    {
        return await _context.StaffMembers
            .Where(s => s.ShortName.Value == name)
            .ToListAsync();
    }

    public async Task<IEnumerable<StaffMember>> GetByQualificationAsync(string qualification)
    {
        var qualificationId = Guid.Parse(qualification);

        var staffMembers = await _context.StaffMembers
            .AsNoTracking()
            .ToListAsync();

        return staffMembers
            .Where(s => s.QualificationIds.Contains(qualificationId));
    }

    public async Task ActivateAsync(Guid id)
    {
        var staffMember = await GetByGuidAsync(id);
        if (staffMember == null)
            throw new KeyNotFoundException($"Staff member with ID '{id}' not found.");

        staffMember.SetAvailability(true);
        await UpdateAsync(staffMember);
    }

    public async Task DeactivateAsync(Guid id)
    {
        var staffMember = await GetByGuidAsync(id);
        if (staffMember == null)
            throw new KeyNotFoundException($"Staff member with ID '{id}' not found.");

        staffMember.SetAvailability(false);
        await UpdateAsync(staffMember);
    }


    public async Task<StaffMember?> GetByGuidAsync(Guid id)
    {
        return await _context.StaffMembers
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task RemoveAsync(Guid id)
    {
        var staffMember = await GetByGuidAsync(id);
        if (staffMember == null)
            throw new KeyNotFoundException($"Staff member with ID '{id}' not found.");
        _context.StaffMembers.Remove(staffMember);
        await _context.SaveChangesAsync();
    }

}
