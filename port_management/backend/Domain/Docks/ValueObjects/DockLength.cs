namespace PortManagement.Domain.Docks.ValueObjects;

public sealed class DockLength
{
    public int Value { get; }

    public DockLength(int value)
    {
        if (value < 0)
            throw new ArgumentException("Dock length cannot be negative", nameof(value));
        Value = value;
    }

    public DockLength(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid length value", nameof(value));
        if (n < 0)
            throw new ArgumentException("Dock length cannot be negative", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(DockLength l) => l.Value;
    public static implicit operator DockLength(int v) => new DockLength(v);
}
