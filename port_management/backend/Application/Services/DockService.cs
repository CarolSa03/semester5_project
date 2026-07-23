using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services;

// This service contains business logic for managing docks
public class DockService : IDockService
{
    private readonly IDockRepository _dockRepo;
    private readonly IVesselTypeRepository _vesselTypeRepo;

    public DockService(IDockRepository dockRepo, IVesselTypeRepository vesselTypeRepo)
    {
        _dockRepo = dockRepo;
        _vesselTypeRepo = vesselTypeRepo;
    }

    private async Task ValidateDock(Dock entity, bool isUpdate = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Name.Value))
            throw new DomainValidationException("Dock name is required");

        if (string.IsNullOrWhiteSpace(entity.Location.Value))
            throw new DomainValidationException("Dock location is required");

        if (entity.Length.Value <= 0)
            throw new DomainValidationException("Dock length must be greater than 0");

        if (entity.Depth.Value <= 0)
            throw new DomainValidationException("Dock depth must be greater than 0");

        if (entity.MaxDraft.Value <= 0)
            throw new DomainValidationException("Max draft must be greater than 0");

        if (entity.MaxDraft.Value > entity.Depth.Value)
            throw new DomainValidationException("Max draft cannot exceed dock depth");

        if (entity.MaxSTS.Value < 0)
            throw new DomainValidationException("Max STS cranes cannot be negative");

        if (entity.AllowedVesselTypes == null || entity.AllowedVesselTypes.Count == 0)
        {
            throw new DomainValidationException("At least one allowed vessel type must be specified");
        }

        if (!isUpdate)
        {
            var existingDock = await _dockRepo.GetByNameAsync(entity.Name.Value, ct);
            if (existingDock != null)
                throw new BusinessRuleViolationException($"Dock with name '{entity.Name.Value}' already exists");
        }
    }
    public async Task<IEnumerable<Dock>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dockRepo.GetAllAsync(ct);

    }

    public async Task<Dock?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dockRepo.GetByIdAsync(id, ct);
    }

    public async Task AddAsync(Dock entity, CancellationToken ct = default)
    {
        entity.Id = Guid.NewGuid();
        await ValidateDock(entity, isUpdate: false, ct);

        await _dockRepo.AddAsync(entity, ct);
        await _dockRepo.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Dock update, CancellationToken ct = default)
    {
        if (update == null)
            throw new ArgumentNullException(nameof(update));

        await ValidateDock(update, isUpdate: true, ct);

        var existingDock = await _dockRepo.GetByIdAsync(update.Id, ct);
        if (existingDock == null)
            throw new KeyNotFoundException($"Dock with ID {update.Id} not found.");


        if (!string.IsNullOrWhiteSpace(update.Name.Value) && update.Name != existingDock.Name)
        {
            var duplicateDock = await _dockRepo.GetByNameAsync(update.Name.Value, ct);
            if (duplicateDock != null && duplicateDock.Id != update.Id)
                throw new BusinessRuleViolationException($"Dock with name '{update.Name}' already exists");
        }

        var changed = false;

        if (!string.IsNullOrWhiteSpace(update.Name.Value))
        {
            existingDock.Name = update.Name;
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(update.Location.Value))
        {
            existingDock.Location = update.Location;
            changed = true;
        }

        if (update.Length.Value != null && update.Length.Value > 0)
        {
            existingDock.Length = update.Length;
            changed = true;
        }

        if (update.Depth != null && update.Depth.Value > 0)
        {
            existingDock.Depth = update.Depth;
            changed = true;
        }

        if (update.MaxDraft != null && update.MaxDraft.Value > 0)
        {
            if (update.MaxDraft.Value > existingDock.Depth.Value)
                throw new DomainValidationException("Max draft cannot exceed dock depth");

            existingDock.MaxDraft = update.MaxDraft;
            changed = true;
        }

        if (update.MaxSTS != null && update.MaxSTS.Value >= 0)
        {
            existingDock.MaxSTS = update.MaxSTS;
            if (existingDock.STSCranes.Count > existingDock.MaxSTS.Value)
                existingDock.STSCranes = existingDock.STSCranes.Take(existingDock.MaxSTS.Value).ToList();
            changed = true;
        }
        if (update.AllowedVesselTypes != null && update.AllowedVesselTypes.Any())
        {
            existingDock.AllowedVesselTypes.Clear();
            existingDock.AllowedVesselTypes.AddRange(update.AllowedVesselTypes);
            changed = true;
        }

        if (update.STSCranes != null)
        {
            existingDock.STSCranes = update.STSCranes.Take(existingDock.MaxSTS.Value).ToList();
            changed = true;
        }
        if (existingDock.IsActive != update.IsActive)
        {
            existingDock.IsActive = update.IsActive;
            changed = true;
        }

        if (!changed)
            existingDock.UpdatedAt = DateTime.UtcNow;
        await _dockRepo.UpdateAsync(existingDock, ct);
        await _dockRepo.SaveChangesAsync(ct);

    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var dock = await _dockRepo.GetByIdAsync(id, ct);
        if (dock == null)
            throw new KeyNotFoundException($"Dock with ID {id} not found.");
        if (dock.IsActive)
        {
            throw new BusinessRuleViolationException("Cannot delete an active dock.");
        }

        dock.IsActive = false;
        dock.UpdatedAt = DateTime.UtcNow;
        await _dockRepo.SaveChangesAsync(ct);
    }
    public async Task<List<Dock>> SearchAsync(string? name = null, string? location = null, Guid? vesselTypeId = null, CancellationToken ct = default)
    {

        var docks = await _dockRepo.SearchAsync(name, location, vesselTypeId, ct);
        return docks.Where(d => d.IsActive).ToList();
    }
}
