using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.PhysicalResources.Enums;
using PortManagement.Domain.PhysicalResources.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.PhysicalResources
{
    public class YardCraneTests
    {
        #region Valid Construction

        [Theory]
        [InlineData("YC-001", "Crane A", "Yard 1", 50, 5, "Mon-Fri 08:00-18:00")]
        [InlineData("YC-002", "Crane B", "Yard 2", 60, 10, "24/7")]
        public void Should_Create_YardCrane_With_Valid_Data(
            string code, string description, string area, double capacity, int setupTime, string operationalWindow)
        {
            // Arrange & Act
            var crane = new YardCrane(
                new PRCode(code),
                new PRDescription(description),
                new PRArea(area),
                new PRSetupTime(setupTime),
                new PROperationalWindow(operationalWindow),
                new PRCapacity(capacity.ToString(), "Containers/Hour")
            );

            // Assert
            Assert.NotNull(crane);
            Assert.Equal(code, crane.Code.Value);
            Assert.Equal(description, crane.Description.Value);
            Assert.Equal(capacity, crane.Capacity.Value);
            Assert.Equal("Containers/Hour", crane.Capacity.Unit);
            Assert.Equal(PRStatus.Active, crane.Status);
        }

        #endregion

        #region Invalid Construction

        [Theory]
        [InlineData("", "Crane", "Zone 1", 25, 5, "Mon-Fri 08:00-18:00")]
        [InlineData("YC-001", "", "Zone 1", 25, 5, "Mon-Fri 08:00-18:00")]
        [InlineData("YC-001", "Crane", "", 25, 5, "Mon-Fri 08:00-18:00")]
        public void Should_Throw_Exception_For_Invalid_YardCrane_Data(
            string code, string description, string area, double capacity, int setupTime, string opWindow)
        {
            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                new YardCrane(
                    new PRCode(code),
                    new PRDescription(description),
                    new PRArea(area),
                    new PRSetupTime(setupTime),
                    new PROperationalWindow(opWindow),
                    new PRCapacity(capacity.ToString(), "Containers/Hour")
                );
            });
        }

        #endregion

        #region Status Tests

        [Fact]
        public void Should_Change_Status()
        {
            // Arrange
            var crane = new YardCrane(
                new PRCode("YC-010"),
                new PRDescription("Yard Crane"),
                new PRArea("Zone 2"),
                new PRSetupTime(12),
                new PROperationalWindow("Mon-Fri 08:00-20:00"),
                new PRCapacity("35", "Containers/Hour")
            );

            // Act & Assert
            crane.MarkInactive();
            Assert.Equal(PRStatus.Inactive, crane.Status);

            crane.MarkMaintenance();
            Assert.Equal(PRStatus.Maintenance, crane.Status);

            crane.MarkActive();
            Assert.Equal(PRStatus.Active, crane.Status);
        }

        #endregion

        #region Update Capacity Tests

        [Theory]
        [InlineData("YC-001", "Yard crane", "Zone 1", 25, 40)]
        [InlineData("YC-002", "Large Yard crane", "Zone 2", 30, 50)]
        public void UpdateYardCrane_ShouldChangeCapacity(
            string code, string desc, string area, double oldCap, double newCap)
        {
            // Arrange
            var crane = new YardCrane(
                new PRCode(code),
                new PRDescription(desc),
                new PRArea(area),
                new PRSetupTime(8),
                new PROperationalWindow("Mon-Fri 06:00-22:00"),
                new PRCapacity(oldCap.ToString(), "Containers/Hour")
            );

            var newCapacity = new PRCapacity(newCap.ToString(), "Containers/Hour");

            // Act
            crane.UpdateCapacity(newCapacity);

            // Assert
            Assert.Equal(newCap, crane.Capacity.Value);
            Assert.Equal("Containers/Hour", crane.Capacity.Unit);
        }

        #endregion
    }
}