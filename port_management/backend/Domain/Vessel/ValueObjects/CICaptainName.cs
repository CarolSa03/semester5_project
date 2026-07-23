namespace PortManagement.Domain.Vessel.ValueObjects;

using System;

public sealed class CICaptainName
{
    public string Value { get; }

    public CICaptainName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Captain name is required", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 200)
            throw new ArgumentException("Captain name length is invalid", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(CICaptainName n) => n.Value;
    public static implicit operator CICaptainName(string value) => new CICaptainName(value);
}
