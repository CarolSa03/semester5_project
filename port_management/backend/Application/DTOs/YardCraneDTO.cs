using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.PhysicalResources.ValueObjects;
using PortManagement.Domain.PhysicalResources.Enums;

namespace PortManagement.Application.DTOs;

public class YardCraneDto : PhysicalResourceDto
{
    public string Capacity { get; set; } = string.Empty;
    public string CapacityUnit { get; set; } = "Containers/Hour";

    public static explicit operator YardCrane(YardCraneDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var id = dto.Id ?? Guid.NewGuid();

        var code = new PRCode(dto.Code);
        var description = new PRDescription(dto.Description);
        var area = new PRArea(dto.Area);
        var setup = new PRSetupTime(dto.SetupTime);
        var window = new PROperationalWindow(dto.OperationalWindow);
        var capacity = new PRCapacity(dto.Capacity, dto.CapacityUnit);

        var status = Enum.TryParse<PRStatus>(dto.Status, out var parsedStatus)
            ? parsedStatus
            : PRStatus.Active;

        var yardCrane = new YardCrane(
            code,
            description,
            area,
            setup,
            window,
            capacity,
            status,
            dto.RequiredQualificationIds
        );

        return yardCrane;
    }

    public static explicit operator YardCraneDto(YardCrane entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new YardCraneDto
        {
            Id = entity.Id,
            Code = entity.Code.Value,
            Description = entity.Description,
            Area = entity.Area,
            SetupTime = entity.SetupTime.Minutes,
            OperationalWindow = entity.OperationalWindow.Value,
            RequiredQualificationIds = entity.RequiredQualificationIds.ToList(),
            Status = entity.Status.ToString(),
            Capacity = entity.Capacity.Value.ToString(),
            CapacityUnit = entity.Capacity.Unit
        };
    }
}
