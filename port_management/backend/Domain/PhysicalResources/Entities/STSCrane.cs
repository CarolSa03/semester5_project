using PortManagement.Domain.PhysicalResources.ValueObjects;
using PortManagement.Domain.PhysicalResources.Enums;

namespace PortManagement.Domain.PhysicalResources.Entities;

public class STSCrane : PhysicalResource
{
    public PRCapacity Capacity { get; set; }

    protected STSCrane() { }

    public STSCrane(
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
