using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.PhysicalResources.ValueObjects;
using PortManagement.Domain.PhysicalResources.Enums;

namespace PortManagement.Application.DTOs;

public class STSCraneDto : PhysicalResourceDto
{
    public string Capacity { get; set; } = string.Empty;
    public string CapacityUnit { get; set; } = "Containers/Hour";

    public static explicit operator STSCrane(STSCraneDto dto)
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

        var sts = new STSCrane(
            code,
            description,
            area,
            setup,
            window,
            capacity,
            status,
            dto.RequiredQualificationIds
        );

        return sts;
    }

    public static explicit operator STSCraneDto(STSCrane entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new STSCraneDto
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
