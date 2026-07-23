using PortManagement.Application.DTOs;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Storage.Entities;
using PortManagement.Domain.Storage.ValueObjects;
using PortManagement.Domain.Storage.Enums;

namespace PortManagement.Application.Mappers;

public class StorageAreaMapper
{
    // Map DTO to Entity (ServedDocks will be populated by the Service layer)
    public static StorageArea ToEntity(StorageAreaDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var saId = new SAId(dto.BusinessId);
        var saLocation = new SALocation(dto.Location);
        var maxCapacity = new SACapacity(dto.MaxCapacityTEU);
        var currentCapacity = new SACapacity(dto.CurrentCapacityTEU);
        var notes = !string.IsNullOrWhiteSpace(dto.Notes) ? new SANotes(dto.Notes) : null;

        // Parse enum type
        if (!Enum.TryParse<SAType>(dto.Type, true, out var saType))
            throw new ArgumentException($"Invalid storage type: {dto.Type}. Must be 'Yard' or 'Warehouse'");

        // ServedDocks will be populated by the service layer
        var storageArea = new StorageArea(saId, saType, saLocation, maxCapacity, currentCapacity, notes, null);
        storageArea.DockDistances = dto.DockDistances;

        // Set Guid Id if provided
        //if (dto.Id.HasValue)
        //    storageArea.SetId(dto.Id.Value);

        return storageArea;
    }

    // Map Entity to DTO
    public static StorageAreaDto ToDto(StorageArea entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new StorageAreaDto
        {
            Id = entity.Id,
            BusinessId = entity.BusinessId.Value,
            Type = entity.Type.ToString(),
            Location = entity.Location.Value,
            MaxCapacityTEU = entity.MaxCapacity.Value,
            CurrentCapacityTEU = entity.CurrentCapacity.Value,
            ServedDocks = entity.ServedDocks?.Select(d => d.Id).ToList(),
            DockDistances = entity.DockDistances,
            Notes = entity.Notes?.Value
        };
    }

    public static List<DockDto>? ToDockDtoList(List<Dock>? docks)
    {
        return docks?.Select(DockMapper.ToDto).ToList();
    }
}
