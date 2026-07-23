namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class VROwner
{
    public string Value { get; }

    public VROwner(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Vessel owner is required", nameof(value));

        if (value.Length < 2)
            throw new ArgumentException("Vessel owner must be at least 2 characters", nameof(value));

        if (value.Length > 120)
            throw new ArgumentException("Vessel owner cannot exceed 120 characters", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Za-z0-9&\-\s\.'()]{2,120}$"))
            throw new ArgumentException("Vessel owner contains invalid characters. Allowed: letters, numbers, spaces, &, -, ., ', and parentheses", nameof(value));

        // Ensure at least one letter exists
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"[A-Za-z]"))
            throw new ArgumentException("Vessel owner must contain at least one letter", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(VROwner owner) => owner.Value;

    public override bool Equals(object? obj)
    {
        if (obj is VROwner other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}

