using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.ShippingAgent.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services;

public class ShippingAgentRepresentativeService : IShippingAgentRepresentativeService
{
    private readonly IShippingAgentRepresentativeRepository _repository;

    public ShippingAgentRepresentativeService(IShippingAgentRepresentativeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ShippingAgentRepresentativeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var representatives = await _repository.GetAllAsync(ct);
        return representatives.Select(ShippingAgentRepresentativeMapper.ToDto).ToList();
    }

    public async Task<ShippingAgentRepresentativeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var representative = await _repository.GetByIdAsync(id, ct);
        return representative == null ? null : ShippingAgentRepresentativeMapper.ToDto(representative);
    }

    public async Task<List<ShippingAgentRepresentativeDto>> GetByOrganizationIdAsync(string organizationId, CancellationToken ct = default)
    {
        var representatives = await _repository.GetByOrganizationIdAsync(organizationId, ct);
        return representatives.Select(ShippingAgentRepresentativeMapper.ToDto).ToList();
    }

    public async Task<ShippingAgentRepresentativeDto> CreateAsync(ShippingAgentRepresentativeDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Email is required");

        if (string.IsNullOrWhiteSpace(dto.Phone))
            throw new ArgumentException("Phone is required");

        if (string.IsNullOrWhiteSpace(dto.ShippingAgentOrganizationId))
            throw new ArgumentException("Shipping Agent Organization ID is required");
        var existingByEmail = await _repository.GetByEmailAsync(dto.Email, ct);
        if (existingByEmail != null)
            throw new InvalidOperationException($"Representative with email {dto.Email} already exists");

        var representative = ShippingAgentRepresentativeMapper.ToEntity(dto);

        await _repository.AddAsync(representative, ct);
        await _repository.SaveChangesAsync(ct);

        dto.Id = representative.Id;
        dto.CreatedAt = representative.CreatedAt;
        return dto;
    }

    public async Task<ShippingAgentRepresentativeDto?> UpdateAsync(int id, ShippingAgentRepresentativeDto dto, CancellationToken ct = default)
    {
        var representative = await _repository.GetByIdAsync(id, ct);
        if (representative == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            representative.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            representative.Email = dto.Email;

        if (!string.IsNullOrWhiteSpace(dto.Phone))
            representative.Phone = dto.Phone;

        await _repository.UpdateAsync(representative, ct);
        await _repository.SaveChangesAsync(ct);

        return ShippingAgentRepresentativeMapper.ToDto(representative);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var representative = await _repository.GetByIdAsync(id, ct);
        if (representative == null) return false;

        await _repository.DeleteAsync(representative, ct);
        await _repository.SaveChangesAsync(ct);
        return true;
    }
}
