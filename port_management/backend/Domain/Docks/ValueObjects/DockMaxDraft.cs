namespace PortManagement.Domain.Docks.ValueObjects;

public sealed class DockMaxDraft
{
    public int Value { get; }

    public DockMaxDraft(int value)
    {
        if (value < 0)
            throw new ArgumentException("Dock max draft cannot be negative", nameof(value));
        Value = value;
    }

    public DockMaxDraft(string value)
    {
        if (!int.TryParse(value, out var n))
            throw new ArgumentException("Invalid max draft value", nameof(value));
        if (n < 0)
            throw new ArgumentException("Dock max draft cannot be negative", nameof(value));
        Value = n;
    }

    public override string ToString() => Value.ToString();
    public static implicit operator int(DockMaxDraft d) => d.Value;
    public static implicit operator DockMaxDraft(int v) => new DockMaxDraft(v);
}
