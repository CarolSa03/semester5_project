using System;

namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class CMContainerId
{
    public string Value { get; }

    public CMContainerId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Container ID is required", nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length < 3 || trimmed.Length > 50)
            throw new ArgumentException("Container ID length is invalid", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(CMContainerId id) => id.Value;
    public static implicit operator CMContainerId(string value) => new CMContainerId(value);
}
