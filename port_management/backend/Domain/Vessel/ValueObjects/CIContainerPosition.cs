namespace PortManagement.Domain.Vessel.ValueObjects;

using System;

public sealed class CIContainerPosition
{
    public int Bay { get; }
    public int Row { get; }
    public int Tier { get; }

    public CIContainerPosition(int bay, int row, int tier)
    {
        if (bay < 0 || row < 0 || tier < 0)
            throw new ArgumentException("Bay, Row and Tier must be non-negative");

        Bay = bay;
        Row = row;
        Tier = tier;
    }

    public override string ToString() => $"Bay:{Bay} Row:{Row} Tier:{Tier}";
}
