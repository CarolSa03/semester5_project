namespace PortManagement.Domain.Vessel.ValueObjects;

using System;
using System.Text.RegularExpressions;

public sealed class CICitizenId
{
    public string Value { get; }

    private static readonly Regex Pattern = new(@"^[A-Za-z0-9\-]{4,30}$", RegexOptions.Compiled);

    public CICitizenId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Citizen ID is required", nameof(value));

        var trimmed = value.Trim();
        if (!Pattern.IsMatch(trimmed))
            throw new ArgumentException("Citizen ID format is invalid", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(CICitizenId id) => id.Value;
    public static implicit operator CICitizenId(string value) => new CICitizenId(value);
}
