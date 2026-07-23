namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public sealed class PRDescription
{
    public string Value { get; }

    public PRDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Description is required", nameof(value));
        if (value.Length < 2)
            throw new ArgumentException("Description too short", nameof(value));
        if (value.Length > 255)
            throw new ArgumentException("Description too long", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(PRDescription d) => d.Value;
    public static implicit operator PRDescription(string value) => new(value);
}
