namespace PortManagement.Domain.Vessel.ValueObjects;

public sealed class VTDescription
{
    public string Value { get; }

    public VTDescription(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (trimmed.Length > 1000)
            throw new ArgumentException("Vessel type description cannot exceed 1000 characters", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(VTDescription d) => d.Value;
    public static implicit operator VTDescription(string value) => new VTDescription(value);
}
