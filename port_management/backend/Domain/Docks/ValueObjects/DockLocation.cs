namespace PortManagement.Domain.Docks.ValueObjects;

public sealed class DockLocation
{
    public string Value { get; }

    public DockLocation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dock location is required", nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length < 1)
            throw new ArgumentException("Dock location is too short", nameof(value));

        if (trimmed.Length > 200)
            throw new ArgumentException("Dock location is too long", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(DockLocation loc) => loc.Value;
    public static implicit operator DockLocation(string value) => new DockLocation(value);
}
