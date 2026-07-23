namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public abstract class ValueUnit
{
    public double Value { get; private set; }
    public string Unit { get; private set; }

    protected ValueUnit(double value, string unit)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be positive");
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit is required");

        Value = value;
        Unit = unit;
    }

    protected ValueUnit(string value, string unit)
    {
        if (!double.TryParse(value, out var numericValue) || numericValue <= 0)
            throw new ArgumentException("Value must be a positive number");
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit is required");

        Value = numericValue;
        Unit = unit;
    }

    public override string ToString() => $"{Value} {Unit}";
}