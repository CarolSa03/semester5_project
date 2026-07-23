namespace PortManagement.Domain.Docks.ValueObjects;

public sealed class DockName
{
    public string Value { get; }

    public DockName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dock name is required", nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length < 1)
            throw new ArgumentException("Dock name is too short", nameof(value));

        if (trimmed.Length > 100)
            throw new ArgumentException("Dock name is too long", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(DockName name) => name.Value;
    public static implicit operator DockName(string value) => new DockName(value);
}
