using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Qualification.Entities;
using PortManagement.Domain.Staff.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services;

public class QualificationService : IQualificationService
{
    private readonly IQualificationRepository _qualificationRepository;

    public QualificationService(IQualificationRepository qualificationRepository)
    {
        _qualificationRepository = qualificationRepository;
    }

    public async Task<IEnumerable<Qualification>> GetAllQualificationsAsync()
    {
        var list = await _qualificationRepository.GetAllAsync();
        return list;
    }

    public async Task<Qualification?> GetQualificationByIdAsync(Guid id)
    {
        var qualification = await _qualificationRepository.GetByIdAsync(id);
        return qualification;
    }

    public async Task<Qualification?> GetQualificationByCodeAsync(string code)
    {
        var qualification = await _qualificationRepository.GetByCodeAsync(code);
        return qualification;
    }

    public async Task<Qualification?> GetQualificationByNameAsync(string name)
    {
        var qualification = await _qualificationRepository.GetByNameAsync(name);
        return qualification;
    }

    public async Task CreateQualificationAsync(Qualification qualification)
    {
        ArgumentNullException.ThrowIfNull(qualification);
        var existing = await _qualificationRepository.GetByCodeAsync(qualification.Code.Value);
        if (existing is not null)
            throw new InvalidOperationException($"A qualification with code '{qualification.Code.Value}' already exists.");

        await _qualificationRepository.AddAsync(qualification);

    }

    public async Task UpdateQualificationAsync(Qualification qualification)
    {
        ArgumentNullException.ThrowIfNull(qualification);
        var existing = await _qualificationRepository.GetByIdAsync(qualification.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Qualification with code {qualification.Code} not found.");

        await _qualificationRepository.UpdateAsync(qualification);
    }

    public async Task DeleteQualificationAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Qualification code is required.", nameof(code));

        var qualification = await _qualificationRepository.GetByCodeAsync(code);
        if (qualification == null)
            throw new KeyNotFoundException($"Qualification with code '{code}' not found.");

        await _qualificationRepository.DeleteAsync(qualification);
    }


}
