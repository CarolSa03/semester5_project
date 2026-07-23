namespace PortManagement.Domain.Qualification.ValueObjects;

public class QDescription
{
    public string Value { get; }

    public QDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Description is required", nameof(value));
        if (value.Length < 2)
            throw new ArgumentException("Description too short", nameof(value));
        if (value.Length > 150)
            throw new ArgumentException("Description too long", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(QDescription d) => d.Value;
    public static implicit operator QDescription(string value) => new(value);
}