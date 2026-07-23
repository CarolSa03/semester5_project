namespace PortManagement.Domain.Storage.ValueObjects;

public sealed class SALocation
{
    public string Value { get; }

    public SALocation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Storage Area location is required", nameof(value));

        if (value.Length < 3)
            throw new ArgumentException("Storage Area location must be at least 3 characters", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Storage Area location cannot exceed 100 characters", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Za-z0-9][A-Za-z0-9\s\-.]{2,99}$"))
            throw new ArgumentException("Storage Area location contains invalid characters. Allowed: letters, numbers, spaces, hyphens, and periods", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(SALocation location) => location.Value;

    public override bool Equals(object? obj)
    {
        if (obj is SALocation other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}

