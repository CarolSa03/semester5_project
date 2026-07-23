using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.PhysicalResources.ValueObjects;
using PortManagement.Domain.PhysicalResources.Enums;

namespace PortManagement.Application.DTOs;

public class TruckDto : PhysicalResourceDto
{
    public string Capacity { get; set; } = string.Empty;
    public string CapacityUnit { get; set; } = "Containers/Trip";
    public string Speed { get; set; } = string.Empty;
    public string SpeedUnit { get; set; } = "Km/h";

    public static explicit operator Truck(TruckDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var id = dto.Id ?? Guid.NewGuid();

        var code = new PRCode(dto.Code);
        var description = new PRDescription(dto.Description);
        var area = new PRArea(dto.Area);
        var setup = new PRSetupTime(dto.SetupTime);
        var window = new PROperationalWindow(dto.OperationalWindow);
        var capacity = new PRCapacity(dto.Capacity, dto.CapacityUnit);
        var speed = new PRSpeed(dto.Speed, dto.SpeedUnit);

        var status = Enum.TryParse<PRStatus>(dto.Status, out var parsedStatus)
            ? parsedStatus
            : PRStatus.Active;

        var truck = new Truck(
            id,
            code,
            description,
            area,
            setup,
            window,
            capacity,
            speed,
            status,
            dto.RequiredQualificationIds
        );

        return truck;
    }

    public static explicit operator TruckDto(Truck entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new TruckDto
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
            CapacityUnit = entity.Capacity.Unit,
            Speed = entity.Speed.Value.ToString(),
            SpeedUnit = entity.Speed.Unit
        };
    }
}
