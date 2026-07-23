namespace PortManagement.Domain.PhysicalResources.ValueObjects;

public sealed class PRSetupTime
{
    public int Minutes { get; }

    public PRSetupTime(int minutes)
    {
        if (minutes < 0) throw new ArgumentException("Setup time cannot be negative", nameof(minutes));
        Minutes = minutes;
    }

    public override string ToString() => $"{Minutes} min";
}
