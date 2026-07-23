namespace PortManagement.Domain.Qualification.ValueObjects;

public class QCode
{
    public string Value { get; }

    public QCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            throw new ArgumentException("Code cannot be empty", nameof(value));
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{2,3}-\d{3,}$"))
            throw new ArgumentException("Code must follow format: XXX-### (e.g., TRK-001)");
        
        if (value.Length > 15)
            throw new ArgumentException("Code cannot exceed 15 characters", nameof(value));

        Value = value.Trim().ToUpper();
    }

    public static implicit operator string(QCode code) => code.Value;

    public override bool Equals(object? obj)
    {
        if (obj is QCode other)
            return Value == other.Value;
        return false;
    }
    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}