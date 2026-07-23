namespace PortManagement.Domain.Vessel.ValueObjects;

using System;

public sealed class CMCargoType
{
    public string Value { get; }

    public CMCargoType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Cargo type is required", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length > 200)
            throw new ArgumentException("Cargo type is too long", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(CMCargoType ct) => ct.Value;
    public static implicit operator CMCargoType(string value) => new CMCargoType(value);
}
