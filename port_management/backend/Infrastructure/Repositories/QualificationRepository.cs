using Microsoft.EntityFrameworkCore;
using PortManagement.Domain.Qualification.Entities;
using PortManagement.Infrastructure.Data;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Infrastructure.Repositories;

public class QualificationRepository : IQualificationRepository
{
    private readonly PortManagementDbContext _context;

    public QualificationRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Qualification qualification)
    {
        await _context.Qualifications.AddAsync(qualification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Qualification qualification)
    {
        _context.Qualifications.Update(qualification);
        await _context.SaveChangesAsync();
    }

    public async Task<Qualification> GetByIdAsync(Guid id)
    {
        return await _context.Qualifications
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Qualification> GetByCodeAsync(string code)
    {
        return await _context.Qualifications
            .FirstOrDefaultAsync(q => q.Code.Value == code);
    }

    public async Task<Qualification> GetByNameAsync(string name)
    {
        return await _context.Qualifications
            .FirstOrDefaultAsync(q => q.Description.Value == name) ?? throw new InvalidOperationException();
    }

    public async Task<List<Qualification>> GetAllAsync()
    {
        return await _context.Qualifications.ToListAsync();
    }

    public async Task DeleteAsync(Qualification qualification)
    {
        _context.Qualifications.Remove(qualification);
        await _context.SaveChangesAsync();
    }


}

