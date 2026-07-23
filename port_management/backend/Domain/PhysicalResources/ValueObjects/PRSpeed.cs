namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public sealed class PRSpeed : ValueUnit
{
    public PRSpeed(double value, string unit) : base(value, unit)
    {
    }

    public PRSpeed(string value, string unit) : base(value, unit)
    {
    }
    public override string ToString() => $"{Value} {Unit}";
}