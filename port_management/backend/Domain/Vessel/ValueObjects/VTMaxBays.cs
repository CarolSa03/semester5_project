namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class VTMaxBays
{
    public int Value { get; }

    public VTMaxBays(int value)
    {
        if (value < 0)
            throw new ArgumentException("MaxBays cannot be negative", nameof(value));
        Value = value;
    }

    public VTMaxBays(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid MaxBays value", nameof(value));
        if (n < 0)
            throw new ArgumentException("MaxBays cannot be negative", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(VTMaxBays v) => v.Value;
    public static implicit operator VTMaxBays(int v) => new VTMaxBays(v);
}
