namespace PortManagement.Domain.Storage.ValueObjects;

public sealed class SAId
{
    public string Value { get; }

    public SAId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Storage Area ID is required", nameof(value));

        if (value.Length < 5 || value.Length > 10)
            throw new ArgumentException("Storage Area ID must be between 5 and 10 characters", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^SA-[A-Z0-9]{2,6}$"))
            throw new ArgumentException("Storage Area ID must follow format: SA-XXX (e.g., SA-01, SA-A1, SA-YD1)", nameof(value));

        Value = value.Trim().ToUpper();
    }

    public static implicit operator string(SAId id) => id.Value;

    public override bool Equals(object? obj)
    {
        if (obj is SAId other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}

