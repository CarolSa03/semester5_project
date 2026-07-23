using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services;

public class ShippingAgentOrganizationService : IShippingAgentOrganizationService
{
    private readonly IShippingAgentOrganizationRepository _repository;

    public ShippingAgentOrganizationService(IShippingAgentOrganizationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ShippingAgentOrganizationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var organizations = await _repository.GetAllAsync(ct);
        return organizations.Select(ShippingAgentOrganizationMapper.ToDto).ToList();
    }

    public async Task<ShippingAgentOrganizationDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var organization = await _repository.GetByIdAsync(id, ct);
        return organization == null ? null : ShippingAgentOrganizationMapper.ToDto(organization);
    }

    public async Task<ShippingAgentOrganizationDto> CreateAsync(ShippingAgentOrganizationDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.LegalName))
            throw new ArgumentException("Legal name is required");

        if (string.IsNullOrWhiteSpace(dto.TaxNumber))
            throw new ArgumentException("Tax number is required");
        var existingByTaxNumber = await _repository.GetByTaxNumberAsync(dto.TaxNumber, ct);
        if (existingByTaxNumber != null)
            throw new InvalidOperationException($"Organization with tax number {dto.TaxNumber} already exists");

        var organization = ShippingAgentOrganizationMapper.ToEntity(dto);

        await _repository.AddAsync(organization, ct);
        await _repository.SaveChangesAsync(ct);

        dto.Id = organization.Id;
        dto.CreatedAt = organization.CreatedAt;
        return dto;
    }

    public async Task<ShippingAgentOrganizationDto?> UpdateAsync(string id, ShippingAgentOrganizationDto dto, CancellationToken ct = default)
    {
        var organization = await _repository.GetByIdAsync(id, ct);
        if (organization == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.LegalName))
            organization.LegalName = dto.LegalName;

        if (!string.IsNullOrWhiteSpace(dto.AlternativeName))
            organization.AlternativeName = dto.AlternativeName;

        if (!string.IsNullOrWhiteSpace(dto.TaxNumber) && dto.TaxNumber != organization.TaxNumber)
        {
            var existingByTaxNumber = await _repository.GetByTaxNumberAsync(dto.TaxNumber, ct);
            if (existingByTaxNumber != null && existingByTaxNumber.Id != id)
                throw new InvalidOperationException($"Tax number {dto.TaxNumber} is already in use");

            organization.TaxNumber = dto.TaxNumber;
        }

        await _repository.UpdateAsync(organization, ct);
        await _repository.SaveChangesAsync(ct);

        return ShippingAgentOrganizationMapper.ToDto(organization);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var organization = await _repository.GetByIdAsync(id, ct);
        if (organization == null) return false;

        await _repository.DeleteAsync(organization, ct);
        await _repository.SaveChangesAsync(ct);
        return true;
    }
}
