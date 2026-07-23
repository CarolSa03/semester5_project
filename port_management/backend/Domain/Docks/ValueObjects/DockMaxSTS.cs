namespace PortManagement.Domain.Docks.ValueObjects;

using System;

public sealed class DockMaxSTS
{
    public int Value { get; }

    public DockMaxSTS(int value)
    {
        if (value < 0)
            throw new ArgumentException("Dock MaxSTS cannot be negative", nameof(value));
        Value = value;
    }

    public DockMaxSTS(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid MaxSTS value", nameof(value));
        if (n < 0)
            throw new ArgumentException("Dock MaxSTS cannot be negative", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(DockMaxSTS d) => d.Value;
    public static implicit operator DockMaxSTS(int v) => new DockMaxSTS(v);
}
