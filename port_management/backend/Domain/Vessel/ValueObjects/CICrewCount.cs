namespace PortManagement.Domain.Vessel.ValueObjects;

using System;

public sealed class CICrewCount
{
    public int Value { get; }

    public CICrewCount(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Crew count must be positive", nameof(value));
        Value = value;
    }

    public CICrewCount(string value)
    {
        if (!int.TryParse(value, out var n) || n <= 0)
            throw new ArgumentException("Invalid crew count", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(CICrewCount ci) => ci.Value;
    public static implicit operator CICrewCount(int v) => new CICrewCount(v);
}
