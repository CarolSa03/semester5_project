namespace PortManagement.Domain.Vessel.ValueObjects;

using System;

public sealed class VTMaxRows
{
    public int Value { get; }

    public VTMaxRows(int value)
    {
        if (value < 0)
            throw new ArgumentException("MaxRows cannot be negative", nameof(value));
        Value = value;
    }

    public VTMaxRows(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid MaxRows value", nameof(value));
        if (n < 0)
            throw new ArgumentException("MaxRows cannot be negative", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(VTMaxRows v) => v.Value;
    public static implicit operator VTMaxRows(int v) => new VTMaxRows(v);
}
