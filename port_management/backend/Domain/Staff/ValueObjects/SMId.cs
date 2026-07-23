namespace PortManagement.Domain.Staff.ValueObjects;

using System;
using System.Text.RegularExpressions;

public class SMId
{
    public string Value { get; }

    private static string _pattern = @"^[A-Za-z0-9\-]+$"; // alfanumérico + hífen

    public SMId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Staff Member ID cannot be null or empty.", nameof(value));

        if (!Regex.IsMatch(value, _pattern))
            throw new ArgumentException($"Staff Member ID '{value}' is invalid. Must match pattern: {_pattern}", nameof(value));
        
        if (value.Length > 20)
            throw new ArgumentException("Staff Member ID cannot exceed 20 characters.", nameof(value));
        
        

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(SMId id) => id.Value;
    public static implicit operator SMId(string value) => new(value);
}