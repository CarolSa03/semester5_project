using FluentAssertions;
using PortManagement.Domain.Storage.Entities;
using PortManagement.Domain.Storage.ValueObjects;
using PortManagement.Domain.Storage.Enums;
using PortManagement.Domain.Docks.Entities;
using Xunit;

namespace PortManagement.Tests.Domain.Entities
{
    /// <summary>
    /// Black box tests for StorageArea entity
    /// </summary>
    public class StorageAreaTest
    {
        #region StorageArea Creation

        [Fact]
        public void CreateStorageArea_WithValidProperties_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var businessId = new SAId("SA-001");
            var type = SAType.Yard;
            var location = new SALocation("Zona Norte");
            var maxCapacity = new SACapacity(1000);
            var currentCapacity = new SACapacity(250);
            var notes = new SANotes("Área coberta");
            var docks = new List<Dock>();
            var dockDistances = new Dictionary<int, double> { { 1, 120.5 } };

            // Act
            var storageArea = new StorageArea(businessId, type, location, maxCapacity, currentCapacity, notes, docks);
            storageArea.DockDistances = dockDistances;

            // Assert
            storageArea.BusinessId.Value.Should().Be("SA-001");
            storageArea.Type.Should().Be(SAType.Yard);
            storageArea.Location.Value.Should().Be("Zona Norte");
            storageArea.MaxCapacity.Value.Should().Be(1000);
            storageArea.CurrentCapacity.Value.Should().Be(250);
            storageArea.ServedDocks.Should().NotBeNull();
            storageArea.Notes.Should().NotBeNull();
            storageArea.Notes!.Value.Should().Be("Área coberta");
            storageArea.DockDistances.Should().BeEquivalentTo(dockDistances);
        }

        [Fact]
        public void CreateStorageArea_WithNullNotes_ShouldWork()
        {
            // Arrange
            var businessId = new SAId("SA-002");
            var type = SAType.Warehouse;
            var location = new SALocation("Zona Sul");
            var maxCapacity = new SACapacity(500);
            var currentCapacity = new SACapacity(100);

            // Act
            var storageArea = new StorageArea(businessId, type, location, maxCapacity, currentCapacity);

            // Assert
            storageArea.Notes.Should().BeNull();
        }

        [Fact]
        public void UpdateCapacity_WithValidCapacity_ShouldUpdateCurrentCapacity()
        {
            // Arrange
            var businessId = new SAId("SA-003");
            var type = SAType.Yard;
            var location = new SALocation("Zona Este");
            var maxCapacity = new SACapacity(1000);
            var currentCapacity = new SACapacity(250);
            var storageArea = new StorageArea(businessId, type, location, maxCapacity, currentCapacity);

            var newCapacity = new SACapacity(500);

            // Act
            storageArea.UpdateCapacity(newCapacity);

            // Assert
            storageArea.CurrentCapacity.Value.Should().Be(500);
        }

        [Fact]
        public void UpdateCapacity_ExceedingMaxCapacity_ShouldThrowArgumentException()
        {
            // Arrange
            var businessId = new SAId("SA-004");
            var type = SAType.Yard;
            var location = new SALocation("Zona Oeste");
            var maxCapacity = new SACapacity(1000);
            var currentCapacity = new SACapacity(250);
            var storageArea = new StorageArea(businessId, type, location, maxCapacity, currentCapacity);

            var tooHighCapacity = new SACapacity(1500);

            // Act & Assert
            Action act = () => storageArea.UpdateCapacity(tooHighCapacity);
            act.Should().Throw<ArgumentException>()
                .WithMessage("Current capacity cannot exceed max capacity");
        }

        [Fact]
        public void CreateStorageArea_CurrentCapacityExceedingMax_ShouldThrowArgumentException()
        {
            // Arrange
            var businessId = new SAId("SA-005");
            var type = SAType.Warehouse;
            var location = new SALocation("Zona Central");
            var maxCapacity = new SACapacity(500);
            var currentCapacity = new SACapacity(600);

            // Act & Assert
            Action act = () => new StorageArea(businessId, type, location, maxCapacity, currentCapacity);
            act.Should().Throw<ArgumentException>()
                .WithMessage("Current capacity cannot exceed max capacity");
        }

        #endregion
    }
}
