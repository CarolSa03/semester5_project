namespace PortManagement.Domain.Storage.ValueObjects;

public sealed class SACapacity
{
    public int Value { get; }

    public SACapacity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Storage Area capacity cannot be negative", nameof(value));

        if (value > 10000)
            throw new ArgumentException("Storage Area capacity cannot exceed 10,000 TEU", nameof(value));

        Value = value;
    }

    public static implicit operator int(SACapacity capacity) => capacity.Value;

    public override bool Equals(object? obj)
    {
        if (obj is SACapacity other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => $"{Value} TEU";
}

