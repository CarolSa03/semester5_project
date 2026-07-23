using System.Text.RegularExpressions;

namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public sealed partial class PRArea
{
    private static readonly Regex Pattern = MyRegex();
    public string Value { get; }

    public PRArea(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Area is required", nameof(value));
        if (!Pattern.IsMatch(value)) throw new ArgumentException("Area contains invalid characters/length", nameof(value));
        Value = value.Trim();
    }

    // Allows letters, numbers, spaces, underscores, hyphens, max length 64
    [GeneratedRegex(@"^[A-Za-z0-9 _\-]{1,64}$")]
    private static partial Regex MyRegex();

    public static implicit operator string(PRArea a) => a.Value;

    public static implicit operator PRArea(string value) => new(value);
    public override string ToString() => Value;
}