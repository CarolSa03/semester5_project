namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class VTMaxTiers
{
    public int Value { get; }

    public VTMaxTiers(int value)
    {
        if (value < 0)
            throw new ArgumentException("MaxTiers cannot be negative", nameof(value));
        Value = value;
    }

    public VTMaxTiers(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid MaxTiers value", nameof(value));
        if (n < 0)
            throw new ArgumentException("MaxTiers cannot be negative", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(VTMaxTiers v) => v.Value;
    public static implicit operator VTMaxTiers(int v) => new VTMaxTiers(v);
}
