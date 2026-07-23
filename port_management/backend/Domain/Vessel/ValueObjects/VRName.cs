namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class VRName
{
    public string Value { get; }

    public VRName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Vessel name is required", nameof(value));

        if (value.Length < 3)
            throw new ArgumentException("Vessel name must be at least 3 characters", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Vessel name cannot exceed 100 characters", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Za-z0-9][A-Za-z0-9\s\-'()]{2,99}$"))
            throw new ArgumentException("Vessel name contains invalid characters. Allowed: letters, numbers, spaces, hyphens, apostrophes, and parentheses", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(VRName name) => name.Value;

    public override bool Equals(object? obj)
    {
        if (obj is VRName other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}

