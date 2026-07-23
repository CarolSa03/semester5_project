using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services;

// This service contains business logic for managing vessel types
public class VesselTypeService : IVesselTypeService
{
    private readonly IVesselTypeRepository _repo;
    private IDockRepository _dockRepo;

    // Constructor injects the vessel type repository
    public VesselTypeService(IVesselTypeRepository repo, IDockRepository dockRepo)
    {
        _repo = repo;
        _dockRepo = dockRepo;
    }

    // Validate vessel type
    private async Task ValidateVesselType(VesselTypeDto dto, bool isUpdate = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new DomainValidationException("Name is required");

        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new DomainValidationException("Description is required");

        if (dto.CapacityTEU <= 0)
            throw new DomainValidationException("Capacity TEU must be greater than 0");

        if (dto.MaxRows <= 0)
            throw new DomainValidationException("Max rows must be greater than 0");

        if (dto.MaxBays <= 0)
            throw new DomainValidationException("Max bays must be greater than 0");

        if (dto.MaxTiers <= 0)
            throw new DomainValidationException("Max tiers must be greater than 0");

        // Check for duplicate name
        if (!isUpdate)
        {
            var existing = await _repo.GetByNameAsync(dto.Name, ct);
            if (existing != null)
                throw new BusinessRuleViolationException($"Vessel type with name '{dto.Name}' already exists");
        }
    }

    // Returns all vessel types
    public async Task<List<VesselTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return list.Select(VesselTypeMapper.ToDto).ToList();
    }

    // Returns active vessel types only
    public async Task<List<VesselTypeDto>> GetActiveAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return list.Where(v => v.IsActive).Select(VesselTypeMapper.ToDto).ToList();
    }

    // Finds a vessel type by its ID
    public async Task<VesselTypeDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var v = await _repo.GetByIdAsync(id, ct);
        return v == null ? null : VesselTypeMapper.ToDto(v);
    }

    // Adds a new vessel type
    public async Task<VesselTypeDto> AddAsync(VesselTypeDto dto, CancellationToken ct = default)
    {
        await ValidateVesselType(dto, isUpdate: false, ct);

        var vesselType = VesselTypeMapper.ToEntity(dto);

        await _repo.AddAsync(vesselType, ct);
        await _repo.SaveChangesAsync(ct);

        dto.Id = vesselType.Id;
        dto.CreatedAt = vesselType.CreatedAt;
        dto.IsActive = vesselType.IsActive;
        return dto;
    }

    // Updates an existing vessel type
    public async Task<VesselTypeDto?> UpdateAsync(Guid id, VesselTypeDto dto, CancellationToken ct = default)
    {
        var v = await _repo.GetByIdAsync(id, ct);
        if (v == null) return null;

        // Check for duplicate name if changing name
        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != v.Name)
        {
            var existing = await _repo.GetByNameAsync(dto.Name, ct);
            if (existing != null && existing.Id != id)
                throw new BusinessRuleViolationException($"Vessel type with name '{dto.Name}' already exists");
        }

        if (dto.Name != null) v.Name = dto.Name;
        if (dto.Description != null) v.Description = dto.Description;
        if (dto.CapacityTEU != null) v.CapacityTEU = dto.CapacityTEU;
        if (dto.MaxRows != null) v.MaxRows = dto.MaxRows;
        if (dto.MaxBays != null) v.MaxBays = dto.MaxBays;
        if (dto.MaxTiers != null) v.MaxTiers = dto.MaxTiers;
        if (dto.IsActive != null) v.IsActive = dto.IsActive;

        v.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync(ct);

        return VesselTypeMapper.ToDto(v);
    }

    // Inactivates a vessel type
    public async Task<VesselTypeDto?> InactivateAsync(Guid id, CancellationToken ct = default)
    {
        var v = await _repo.GetByIdAsync(id, ct);
        if (v == null) return null;

        v.IsActive = false;
        v.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync(ct);

        return VesselTypeMapper.ToDto(v);
    }

    // Deletes a vessel type (only if inactive)
    public async Task<VesselTypeDto?> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var v = await _repo.GetByIdAsync(id, ct);
        if (v == null) return null;

        if (v.IsActive)
            throw new BusinessRuleViolationException("Cannot delete an active vessel type.");

        // TODO: Add check if vessel type is in use by docks
        // var docksUsingType = await _dockRepo.GetByVesselTypeAsync(id, ct);
        // if (docksUsingType.Any())
        //     throw new BusinessRuleViolationException("Cannot delete vessel type that is in use by docks");

        await _repo.RemoveAsync(v, ct);
        await _repo.SaveChangesAsync(ct);

        return VesselTypeMapper.ToDto(v);
    }

    // Searches vessel types by name or description
    public async Task<List<VesselTypeDto>> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(ct);

        var results = await _repo.SearchAsync(searchTerm, ct);
        return results.Select(VesselTypeMapper.ToDto).ToList();
    }
}
