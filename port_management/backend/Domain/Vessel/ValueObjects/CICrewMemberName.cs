namespace PortManagement.Domain.Vessel.ValueObjects;

using System;

public sealed class CICrewMemberName
{
    public string Value { get; }

    public CICrewMemberName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Crew member name is required", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 200)
            throw new ArgumentException("Crew member name length is invalid", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(CICrewMemberName n) => n.Value;
    public static implicit operator CICrewMemberName(string value) => new CICrewMemberName(value);
}
