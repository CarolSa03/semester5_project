using PortManagement.Application.Common.Interfaces;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Docks.ValueObjects;
using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.Vessel.Entities;

namespace PortManagement.Application.Mappers;

public class DockMapper
{
    private readonly IVesselTypeRepository _vesselTypeRepo;

    public DockMapper(IVesselTypeRepository vesselTypeRepo)
    {
        _vesselTypeRepo = vesselTypeRepo;
    }
    public static DockDto ToDto(Dock entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        return entity switch
        {
            Dock dock => (DockDto)dock,
            _ => throw new ArgumentException("Unknown entity type", nameof(entity))
        };
    }

    public async Task<Dock> ToEntityAsync(DockDto dto, CancellationToken ct = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var vesselTypes = await _vesselTypeRepo.GetByIdsAsync(dto.AllowedVesselTypes, ct);

        if (vesselTypes.Count != dto.AllowedVesselTypes.Count)
        {
            var foundIds = vesselTypes.Select(vt => vt.Id).ToList();
            var missingIds = dto.AllowedVesselTypes.Except(foundIds).ToList();
            throw new DomainValidationException(
                $"Vessel types not found: {string.Join(", ", missingIds)}");
        }

        return new Dock
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Name = new DockName(dto.Name),
            Location = new DockLocation(dto.Location),
            Length = new DockLength(dto.Length),
            Depth = new DockDepth(dto.Depth),
            MaxDraft = new DockMaxDraft(dto.MaxDraft),
            MaxSTS = new DockMaxSTS(dto.MaxSTS),
            AllowedVesselTypes = vesselTypes,
            STSCranes = new List<STSCrane>(),
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}
