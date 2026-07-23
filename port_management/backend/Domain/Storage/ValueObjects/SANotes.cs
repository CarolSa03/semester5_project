namespace PortManagement.Domain.Storage.ValueObjects;

public sealed class SANotes
{
    public string Value { get; }

    public SANotes(string? value)
    {
        if (value != null && value.Length > 500)
            throw new ArgumentException("Storage Area notes cannot exceed 500 characters", nameof(value));

        Value = value?.Trim() ?? string.Empty;
    }

    public static implicit operator string(SANotes notes) => notes.Value;

    public override bool Equals(object? obj)
    {
        if (obj is SANotes other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}

