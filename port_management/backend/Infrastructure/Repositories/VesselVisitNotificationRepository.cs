using Microsoft.EntityFrameworkCore;
using PortManagement.Application.Common.Interfaces;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Infrastructure.Data;

namespace PortManagement.Infrastructure.Repositories;

public class VesselVisitNotificationRepository : IVesselVisitNotificationRepository
{
    private readonly PortManagementDbContext _context;

    public VesselVisitNotificationRepository(PortManagementDbContext context)
    {
        _context = context;
    }

    public async Task<VesselVisitNotification?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.VesselVisitNotifications
            .Include(v => v.Vessel)
            .Include(v => v.ShippingAgentRepresentative)
            .Include(v => v.CargoManifests)
            .Include(v => v.Crew)
            .Include(v => v.AssignedDock)
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<List<VesselVisitNotification>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.VesselVisitNotifications
            .Include(v => v.Vessel)
            .Include(v => v.ShippingAgentRepresentative)
            .Include(v => v.AssignedDock)
            .ToListAsync(ct);
    }

    public async Task<VesselVisitNotification?> GetByBusinessIdAsync(VesselVisitBusinessId businessId, CancellationToken ct = default)
    {
        return await _context.VesselVisitNotifications
            .Include(v => v.Vessel)
            .Include(v => v.ShippingAgentRepresentative)
            .Include(v => v.CargoManifests)
            .Include(v => v.Crew)
            .Include(v => v.AssignedDock)
            // Acessa o backing field diretamente
            .FirstOrDefaultAsync(v => EF.Property<string>(v, "_businessId") == businessId.Value, ct);
    }

    public async Task<int> GetNextSequentialNumberAsync(int year, string portCode, CancellationToken ct = default)
    {
        var prefix = $"{year}-{portCode}-";

        // Busca usando o backing field
        var businessIds = await _context.VesselVisitNotifications
            .Where(n => EF.Functions.Like(EF.Property<string>(n, "_businessId"), $"{prefix}%"))
            .Select(n => EF.Property<string>(n, "_businessId"))
            .ToListAsync(ct);

        if (!businessIds.Any())
            return 1;

        // Processa em memória
        var maxSeq = businessIds
            .Select(bid =>
            {
                var parts = bid.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var num))
                    return num;
                return 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        return maxSeq + 1;
    }

    public async Task<bool> BusinessIdExistsAsync(VesselVisitBusinessId businessId, CancellationToken ct = default)
    {
        return await _context.VesselVisitNotifications
            .AnyAsync(v => EF.Property<string>(v, "_businessId") == businessId.Value, ct);
    }

    public async Task<IEnumerable<VesselVisitNotification>> GetByShippingAgentAsync(int agentId, CancellationToken ct = default)
    {
        return await _context.VesselVisitNotifications
            .Include(v => v.Vessel)
            .Include(v => v.AssignedDock)
            .Where(v => v.ShippingAgentRepresentative != null &&
                        v.ShippingAgentRepresentative.Id == agentId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<VesselVisitNotification>> GetByStatusAsync(VesselNotificationStatus status, CancellationToken ct = default)
    {
        return await _context.VesselVisitNotifications
            .Include(v => v.Vessel)
            .Include(v => v.ShippingAgentRepresentative)
            .Include(v => v.AssignedDock)
            .Where(v => v.Status == status)
            .ToListAsync(ct);
    }

    public async Task<VesselVisitNotification> AddAsync(VesselVisitNotification notification, CancellationToken ct = default)
    {
        await _context.VesselVisitNotifications.AddAsync(notification, ct);
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(notification.Id, ct) ?? notification;
    }

    public async Task UpdateAsync(VesselVisitNotification notification, CancellationToken ct = default)
    {
        _context.VesselVisitNotifications.Update(notification);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(VesselVisitNotification notification, CancellationToken ct = default)
    {
        _context.VesselVisitNotifications.Remove(notification);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<VesselVisitNotification>> GetByDockAndTimeRangeAsync(
        Guid dockId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken ct = default)
    {
        return await _context.VesselVisitNotifications
            .Include(v => v.Vessel)
            .Include(v => v.AssignedDock)
            .Where(v =>
                v.AssignedDock != null &&
                v.AssignedDock.Id == dockId &&
                v.ETA < endTime &&
                v.ETD > startTime)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<VesselVisitNotification>> SearchForAgentAsync(
        int representativeId,
        VesselNotificationStatus? status,
        Guid? vesselId,
        DateTime? searchPeriodStart,
        DateTime? searchPeriodEnd,
        CancellationToken ct = default)
    {
        var query = _context.VesselVisitNotifications
            .Include(v => v.Vessel)
            .Include(v => v.ShippingAgentRepresentative)
            .Include(v => v.AssignedDock)
            .AsQueryable();

        if (representativeId > 0)
        {
            query = query.Where(v => v.ShippingAgentRepresentative != null &&
                                     v.ShippingAgentRepresentative.Id == representativeId);
        }

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (vesselId != null)
        {
            query = query.Where(v => v.Vessel != null && v.Vessel.Id == vesselId);
        }

        if (searchPeriodStart.HasValue)
        {
            query = query.Where(v => v.ETA >= searchPeriodStart.Value);
        }

        if (searchPeriodEnd.HasValue)
        {
            query = query.Where(v => v.ETD <= searchPeriodEnd.Value);
        }

        return await query.OrderByDescending(v => v.CreatedAt).ToListAsync(ct);
    }
}