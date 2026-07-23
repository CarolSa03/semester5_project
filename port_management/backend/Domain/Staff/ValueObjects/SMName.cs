namespace PortManagement.Domain.Staff.ValueObjects;

public class SMName
{
    public string Value { get; }

    public SMName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Staff Member Name cannot be null or empty", nameof(value));
        
        if (value.Length < 2)
            throw new ArgumentException("Staff Member Name is too short", nameof(value));
        
        if (value.Length > 100)
            throw new ArgumentException("Staff Member Name is too long", nameof(value));
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-ZÀ-ÿ\s'-]+$"))
            throw new ArgumentException("Staff Member Name contains invalid characters", nameof(value));
        
        Value = value;
    }

    public override string ToString() => Value;
    public static implicit operator string(SMName name) => name.Value;
    public static implicit operator SMName(string value) => new(value);
}