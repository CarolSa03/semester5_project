using System;
using System.Text.RegularExpressions;

namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class CINationality
{
    public string Value { get; }

    private static readonly Regex Pattern = new(@"^[A-Za-z\s'-]{2,100}$", RegexOptions.Compiled);

    public CINationality(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Nationality is required", nameof(value));

        var trimmed = value.Trim();
        if (!Pattern.IsMatch(trimmed))
            throw new ArgumentException("Nationality contains invalid characters", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(CINationality n) => n.Value;
    public static implicit operator CINationality(string value) => new CINationality(value);
}
