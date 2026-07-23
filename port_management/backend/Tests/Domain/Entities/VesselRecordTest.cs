using FluentAssertions;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Domain.Vessel.Enums;
using Xunit;

namespace PortManagement.Tests.Domain.Entities
{
    /// <summary>
    /// Black box tests for VesselRecord entity
    /// </summary>
    public class VesselRecordTest
    {
        #region VesselRecord Creation

        [Fact]
        public void CreateVesselRecord_WithValidProperties_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var name = new VRName("Vessel Alpha");
            var owner = new VROwner("Empresa X");

            // Act
            var vesselRecord = new VesselRecord(imo, name, vesselType, owner, VRStatus.Active);

            // Assert
            vesselRecord.Imo.Should().Be(imo);
            vesselRecord.Name.Value.Should().Be("Vessel Alpha");
            vesselRecord.VesselType.Should().Be(vesselType);
            vesselRecord.Owner.Value.Should().Be("Empresa X");
            vesselRecord.Status.Should().Be(VRStatus.Active);
            vesselRecord.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            vesselRecord.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void CreateVesselRecord_WithEmptyName_ShouldThrowException()
        {
            // Arrange
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var owner = new VROwner("Empresa X");

            // Act & Assert
            Action act = () => new VRName(string.Empty);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateVesselRecord_WithEmptyOwner_ShouldThrowException()
        {
            // Arrange
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var name = new VRName("Vessel Alpha");

            // Act & Assert
            Action act = () => new VROwner(string.Empty);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateVesselRecord_WithNullVesselType_ShouldThrowException()
        {
            // Arrange
            var imo = new Imo(1234567);
            var name = new VRName("Vessel Alpha");
            var owner = new VROwner("Empresa X");

            // Act & Assert
            Action act = () => new VesselRecord(imo, name, null!, owner);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateVesselRecord_WithInactiveStatus_ShouldSetStatusToInactive()
        {
            // Arrange
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var name = new VRName("Vessel Alpha");
            var owner = new VROwner("Empresa X");

            // Act
            var vesselRecord = new VesselRecord(imo, name, vesselType, owner, VRStatus.Inactive);

            // Assert
            vesselRecord.Status.Should().Be(VRStatus.Inactive);
        }

        [Fact]
        public void UpdateVesselRecord_MarkActive_ShouldUpdateStatus()
        {
            // Arrange
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var name = new VRName("Vessel Alpha");
            var owner = new VROwner("Empresa X");
            var vesselRecord = new VesselRecord(imo, name, vesselType, owner, VRStatus.Inactive);

            // Act
            vesselRecord.MarkActive();

            // Assert
            vesselRecord.Status.Should().Be(VRStatus.Active);
            vesselRecord.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateVesselRecord_MarkInactive_ShouldUpdateStatus()
        {
            // Arrange
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var name = new VRName("Vessel Alpha");
            var owner = new VROwner("Empresa X");
            var vesselRecord = new VesselRecord(imo, name, vesselType, owner, VRStatus.Active);

            // Act
            vesselRecord.MarkInactive();

            // Assert
            vesselRecord.Status.Should().Be(VRStatus.Inactive);
            vesselRecord.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void Validate_WithValidVesselRecord_ShouldNotThrow()
        {
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var name = new VRName("Vessel Alpha");
            var owner = new VROwner("Empresa X");
            var vesselRecord = new VesselRecord(imo, name, vesselType, owner);

            Action act = () => vesselRecord.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void UpdateFields_ShouldUpdateProperties()
        {
            // Arrange
            var imo = new Imo(1234567);
            var vesselType = CreateValidVesselType();
            var name = new VRName("Vessel Alpha");
            var owner = new VROwner("Empresa X");
            var vesselRecord = new VesselRecord(imo, name, vesselType, owner);

            var newName = new VRName("Vessel Beta");
            var newOwner = new VROwner("Empresa Y");

            // Act
            vesselRecord.UpdateFields(name: newName, owner: newOwner, status: VRStatus.Inactive);

            // Assert
            vesselRecord.Name.Value.Should().Be("Vessel Beta");
            vesselRecord.Owner.Value.Should().Be("Empresa Y");
            vesselRecord.Status.Should().Be(VRStatus.Inactive);
            vesselRecord.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region Helper Methods

        private VesselType CreateValidVesselType()
        {
            return new VesselType
            {
                Id = Guid.NewGuid(),
                Name = "Bulk Carrier",
                Description = "Navio graneleiro",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        #endregion
    }
}
