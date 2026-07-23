
namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public sealed class PRCapacity : ValueUnit
{
    public PRCapacity(double value, string unit) : base(value, unit)
    {
        if (Value <= 0)
            throw new ArgumentException("Capacity must be greater than zero");
    }

    public PRCapacity(string value, string unit) : base(value, unit)
    {
        if (Value <= 0)
            throw new ArgumentException("Capacity must be greater than zero");
    }
    public override string ToString() => $"{Value} {Unit}";
}
