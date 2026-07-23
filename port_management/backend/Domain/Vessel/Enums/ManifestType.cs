namespace PortManagement.Domain.Vessel.Enums;

public enum ManifestType
{
    Unloading, // Cargo being unloaded from the vessel
    Loading // Cargo being loaded onto the vessel
}

//from string to ManifestType
public static class ManifestTypeExtensions
{

    public static ManifestType ToManifestType(this string value)
    {
        return Enum.Parse<ManifestType>(value);
    }

    public static string ToString(this ManifestType value)
    {
        return value.ToString();
    }
}
