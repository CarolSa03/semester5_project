namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class VTCapacityTEU
{
    public int Value { get; }

    public VTCapacityTEU(int value)
    {
        if (value < 0)
            throw new ArgumentException("CapacityTEU cannot be negative", nameof(value));
        Value = value;
    }

    public VTCapacityTEU(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid CapacityTEU value", nameof(value));
        if (n < 0)
            throw new ArgumentException("CapacityTEU cannot be negative", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(VTCapacityTEU c) => c.Value;
    public static implicit operator VTCapacityTEU(int v) => new VTCapacityTEU(v);
}
