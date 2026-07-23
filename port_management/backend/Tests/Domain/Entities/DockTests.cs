using FluentAssertions;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Docks.ValueObjects;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.PhysicalResources.Entities;
using Xunit;

namespace PortManagement.Tests.Domain.Entities;

/// <summary>
/// Black box tests for Dock entity
/// Tests dock validation and properties with new value objects structure
/// </summary>
public class DockTests
{
    #region Dock Creation Tests

    [Fact]
    public void CreateDock_WithValidProperties_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var dockId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var dock = new Dock
        {
            Id = dockId,
            Name = new DockName("Dock A"),
            Location = new DockLocation("North Side"),
            Length = new DockLength(300),
            Depth = new DockDepth(12),
            MaxDraft = new DockMaxDraft(10),
            AllowedVesselTypes = new List<VesselType>(),
            MaxSTS = new DockMaxSTS(2),
            STSCranes = new List<STSCrane>(),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = null
        };

        // Assert
        dock.Id.Should().Be(dockId);
        dock.Name.Value.Should().Be("Dock A");
        dock.Location.Value.Should().Be("North Side");
        dock.Length.Value.Should().Be(300);
        dock.Depth.Value.Should().Be(12);
        dock.MaxDraft.Value.Should().Be(10);
        dock.AllowedVesselTypes.Should().NotBeNull();
        dock.MaxSTS.Value.Should().Be(2);
        dock.IsActive.Should().BeTrue();
        dock.CreatedAt.Should().Be(now);
        dock.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void CreateDock_WithDifferentLength_ShouldSetCorrectValue()
    {
        // Arrange & Act
        var dock = new Dock
        {
            Id = Guid.NewGuid(),
            Name = new DockName("Test Dock"),
            Location = new DockLocation("Test Location"),
            Length = new DockLength(500),
            Depth = new DockDepth(15),
            MaxDraft = new DockMaxDraft(12),
            MaxSTS = new DockMaxSTS(3),
            AllowedVesselTypes = new List<VesselType>(),
            STSCranes = new List<STSCrane>()
        };

        // Assert
        dock.Length.Value.Should().Be(500);
    }

    [Fact]
    public void UpdateDock_UpdatedAt_ShouldChangeUpdatedAt()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dock = new Dock
        {
            Id = Guid.NewGuid(),
            Name = new DockName("Dock A"),
            Location = new DockLocation("North Side"),
            Length = new DockLength(300),
            Depth = new DockDepth(12),
            MaxDraft = new DockMaxDraft(10),
            MaxSTS = new DockMaxSTS(2),
            AllowedVesselTypes = new List<VesselType>(),
            STSCranes = new List<STSCrane>(),
            CreatedAt = now
        };
        
        var updateTime = now.AddHours(1);

        // Act
        dock.UpdatedAt = updateTime;

        // Assert
        dock.UpdatedAt.Should().Be(updateTime);
    }

    #endregion

    #region Value Object Validation Tests

    [Fact]
    public void DockName_WithEmptyString_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new DockName("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DockLocation_WithValidString_ShouldCreateValueObject()
    {
        // Act
        var location = new DockLocation("Test Location");

        // Assert
        location.Value.Should().Be("Test Location");
    }

    [Fact]
    public void DockLength_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new DockLength(-1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DockDepth_WithZeroValue_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new DockDepth(0);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DockMaxDraft_WithValidValue_ShouldCreateValueObject()
    {
        // Act
        var maxDraft = new DockMaxDraft(15);

        // Assert
        maxDraft.Value.Should().Be(15);
    }

    [Fact]
    public void DockMaxSTS_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => new DockMaxSTS(-1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region IsActive and Soft Delete Tests

    [Fact]
    public void CreateDock_IsActiveDefaultValue_ShouldBeTrue()
    {
        // Arrange & Act
        var dock = new Dock
        {
            Id = Guid.NewGuid(),
            Name = new DockName("Dock A"),
            Location = new DockLocation("North Side"),
            Length = new DockLength(300),
            Depth = new DockDepth(12),
            MaxDraft = new DockMaxDraft(10),
            MaxSTS = new DockMaxSTS(2),
            AllowedVesselTypes = new List<VesselType>(),
            STSCranes = new List<STSCrane>()
        };

        // Assert
        dock.IsActive.Should().BeTrue();
    }

    [Fact]
    public void DeactivateDock_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var dock = new Dock
        {
            Id = Guid.NewGuid(),
            Name = new DockName("Dock A"),
            Location = new DockLocation("North Side"),
            Length = new DockLength(300),
            Depth = new DockDepth(12),
            MaxDraft = new DockMaxDraft(10),
            MaxSTS = new DockMaxSTS(2),
            AllowedVesselTypes = new List<VesselType>(),
            STSCranes = new List<STSCrane>(),
            IsActive = true
        };

        // Act
        dock.IsActive = false;

        // Assert
        dock.IsActive.Should().BeFalse();
    }

    #endregion
}
