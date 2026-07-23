namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public sealed class PRCode
{
    public string Value { get; }

    public PRCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Code cannot be empty", nameof(value));
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{2,3}-\d{3,}$"))
            throw new ArgumentException("Code must follow format: XXX-### (e.g., TRK-001)");

        Value = value.Trim().ToUpper();
    }

    public static implicit operator string(PRCode code) => code.Value;

    public override bool Equals(object? obj)
    {
        if (obj is PRCode other)
            return Value == other.Value;
        return false;
    }
    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}