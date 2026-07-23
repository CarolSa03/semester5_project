namespace PortManagement.Domain.Docks.ValueObjects;

using System;

public sealed class DockDepth
{
    public int Value { get; }

    public DockDepth(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Dock depth must be positive", nameof(value));
        Value = value;
    }

    public DockDepth(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid depth value", nameof(value));
        if (n <= 0)
            throw new ArgumentException("Dock depth must be positive", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(DockDepth d) => d.Value;
    public static implicit operator DockDepth(int v) => new DockDepth(v);
}
