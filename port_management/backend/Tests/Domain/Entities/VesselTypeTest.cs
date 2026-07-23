using FluentAssertions;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.Entities
{
    public class VesselTypeTest
    {
        [Fact]
        public void CreateVesselType_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange & Act
            var vesselType = new VesselType
            {
                Id = Guid.NewGuid(),
                Name = new VTName("Bulk Carrier"),
                Description = new VTDescription("Navio graneleiro"),
                CapacityTEU = new VTCapacityTEU(5000),
                MaxRows = new VTMaxRows(10),
                MaxBays = new VTMaxBays(20),
                MaxTiers = new VTMaxTiers(8),
                IsActive = true
            };

            // Assert
            vesselType.Id.Should().NotBeEmpty();
            vesselType.Name.Value.Should().Be("Bulk Carrier");
            vesselType.Description.Value.Should().Be("Navio graneleiro");
            vesselType.CapacityTEU.Value.Should().Be(5000);
            vesselType.MaxRows.Value.Should().Be(10);
            vesselType.MaxBays.Value.Should().Be(20);
            vesselType.MaxTiers.Value.Should().Be(8);
            vesselType.IsActive.Should().BeTrue();
            vesselType.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CreateVesselType_ShouldGenerateId()
        {
            // Arrange & Act
            var vesselType = new VesselType
            {
                Id = Guid.NewGuid(),
                Name = new VTName("Container Ship"),
                Description = new VTDescription("Large container vessel"),
                CapacityTEU = new VTCapacityTEU(10000),
                MaxRows = new VTMaxRows(15),
                MaxBays = new VTMaxBays(25),
                MaxTiers = new VTMaxTiers(10)
            };

            // Assert
            vesselType.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateVesselType_ShouldBeActiveByDefault()
        {
            // Arrange & Act
            var vesselType = new VesselType
            {
                Name = new VTName("Tanker"),
                Description = new VTDescription("Oil tanker"),
                CapacityTEU = new VTCapacityTEU(3000),
                MaxRows = new VTMaxRows(8),
                MaxBays = new VTMaxBays(15),
                MaxTiers = new VTMaxTiers(6)
            };

            // Assert
            vesselType.IsActive.Should().BeTrue();
        }
    }
}
