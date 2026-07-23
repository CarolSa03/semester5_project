using System.Text.RegularExpressions;
using PortManagement.Domain.PhysicalResources.ValueObjects;

namespace PortManagement.Domain.Staff.ValueObjects;

public sealed class SMOperationalWindow
{
    private static readonly Regex Pattern = new(@"^(24/7|([A-Za-z]{3}(-[A-Za-z]{3})?\s+\d{2}:\d{2}-\d{2}:\d{2}))$");
    public string Value { get; }

    public SMOperationalWindow(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Operational window is required", nameof(value));
        if (!Pattern.IsMatch(value)) throw new ArgumentException("Operational window must be like 'Mon-Fri 08:00-20:00' or '24/7'", nameof(value));
        Value = value.Trim();
    }

    public static implicit operator string(SMOperationalWindow v) => v.Value;
    public override string ToString() => Value;
}