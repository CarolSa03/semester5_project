using System.Text.RegularExpressions;

namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public sealed class PROperationalWindow
{
    private static readonly Regex Pattern = new(@"^(24/7|([A-Za-z]{3}(-[A-Za-z]{3})?\s+\d{2}:\d{2}-\d{2}:\d{2}))$");
    public string Value { get; }

    public PROperationalWindow(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Operational window is required", nameof(value));
        if (!Pattern.IsMatch(value)) throw new ArgumentException("Operational window must be like 'Mon-Fri 08:00-20:00' or '24/7'", nameof(value));
        Value = value.Trim();
    }

    public static implicit operator string(PROperationalWindow v) => v.Value;
    public override string ToString() => Value;
}
