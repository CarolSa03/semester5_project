namespace PortManagement.Domain.Vessel.ValueObjects;

using System;

public sealed class VTName
{
    public string Value { get; }

    public VTName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Vessel type name is required", nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length < 3)
            throw new ArgumentException("Vessel type name must be at least 3 characters", nameof(value));

        if (trimmed.Length > 100)
            throw new ArgumentException("Vessel type name cannot exceed 100 characters", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(VTName name) => name.Value;
    public static implicit operator VTName(string value) => new VTName(value);
}
