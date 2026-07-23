using PortManagement.Domain.PhysicalResources.ValueObjects;
using PortManagement.Domain.PhysicalResources.Enums;

namespace PortManagement.Domain.PhysicalResources.Entities;

public class Truck : PhysicalResource
{
    public PRCapacity Capacity { get; set; }
    public PRSpeed Speed { get; set; }

    protected Truck() { }

    public Truck(
        PRCode code,
        PRDescription description,
        PRArea area,
        PRSetupTime setupTime,
        PROperationalWindow operationalWindow,
        PRCapacity capacity,
        PRSpeed speed,
        PRStatus status = PRStatus.Active,
        IEnumerable<Guid>? qualifications = null)
        : base(code, description, area, setupTime, operationalWindow, qualifications, status)
    {
        Capacity = capacity ?? new PRCapacity("0", "Containers/Trip");
        Speed = speed ?? new PRSpeed("0", "Km/h");
    }

    public Truck(
        Guid id,
        PRCode code, PRDescription description, PRArea area, 
        PRSetupTime setupTime, PROperationalWindow operationalWindow, 
        PRCapacity capacity, PRSpeed speed, 
        PRStatus status = PRStatus.Active, IEnumerable<Guid>? qualifications = null)
        : base(id, code, description, area, setupTime, operationalWindow, qualifications, status)
    {
        Capacity = capacity ?? new PRCapacity("0", "Containers/Trip");
        Speed = speed ?? new PRSpeed("0", "Km/h");
    }

    public void UpdateCapacity(PRCapacity newCapacity)
    {
        Capacity = newCapacity ?? throw new ArgumentNullException(nameof(newCapacity));
    }

    public void UpdateSpeed(PRSpeed newSpeed)
    {
        Speed = newSpeed ?? throw new ArgumentNullException(nameof(newSpeed));
    }
}
