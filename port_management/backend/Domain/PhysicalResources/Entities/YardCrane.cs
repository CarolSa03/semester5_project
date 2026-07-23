using PortManagement.Domain.PhysicalResources.Enums;
using PortManagement.Domain.PhysicalResources.ValueObjects;

namespace PortManagement.Domain.PhysicalResources.Entities;

public class YardCrane : PhysicalResource
{
    public PRCapacity Capacity { get; set; }

    protected YardCrane() { }

    public YardCrane(
        PRCode code,
        PRDescription description,
        PRArea area,
        PRSetupTime setupTime,
        PROperationalWindow operationalWindow,
        PRCapacity capacity,
        PRStatus status = PRStatus.Active,
        IEnumerable<Guid>? qualifications = null)
        : base(code, description, area, setupTime, operationalWindow, qualifications, status)
    {
        Capacity = capacity ?? new PRCapacity("0", "Containers/Hours");
    }
    public void UpdateCapacity(PRCapacity newCapacity)
    {
        Capacity = newCapacity ?? throw new ArgumentNullException(nameof(newCapacity));
    }
}
