using System.Text.Json.Serialization;
namespace PortManagement.Application.DTOs;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "resourceType")]
[JsonDerivedType(typeof(TruckDto), typeDiscriminator: "truck")]
[JsonDerivedType(typeof(YardCraneDto), typeDiscriminator: "yardcrane")]
[JsonDerivedType(typeof(STSCraneDto), typeDiscriminator: "stscrane")]
public abstract class PhysicalResourceDto
{
    public Guid? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public int SetupTime { get; set; }
    public string OperationalWindow { get; set; } = "24/7";
    public List<Guid> RequiredQualificationIds { get; set; } = new();
    public string Status { get; set; } = string.Empty;
}
