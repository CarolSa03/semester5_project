using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.PhysicalResources.Enums;
using PortManagement.Domain.PhysicalResources.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.PhysicalResources
{
    public class STSCraneTests
    {
        #region Valid Construction  

        [Theory]
        [InlineData("ST-001", "STS Crane", "Pier A", 30, 10, "Mon-Fri 08:00-18:00")]
        [InlineData("ST-002", "Large STS Crane", "Pier B", 50, 15, "24/7")]
        public void CreateSTSCrane_ShouldInitializeCorrectly(
            string code, string description, string area, double capacity, int setupTime, string operationalWindow)
        {
            // Arrange & Act
            var crane = new STSCrane(
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
            Assert.Equal(area, crane.Area.Value);
            Assert.Equal(capacity, crane.Capacity.Value);
            Assert.Equal("Containers/Hour", crane.Capacity.Unit);
            Assert.Equal(PRStatus.Active, crane.Status);
        }

        #endregion

        #region Invalid Construction

        [Theory]
        [InlineData("", "STS Crane", "Pier A", 30, 10, "Mon-Fri 08:00-18:00")]
        [InlineData("ST-001", "", "Pier A", 30, 10, "Mon-Fri 08:00-18:00")]
        [InlineData("ST-001", "STS Crane", "", 30, 10, "Mon-Fri 08:00-18:00")]
        public void CreateSTSCrane_WithInvalidData_ShouldThrow(
            string code, string desc, string area, double cap, int setupTime, string opWindow)
        {
            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                new STSCrane(
                    new PRCode(code),
                    new PRDescription(desc),
                    new PRArea(area),
                    new PRSetupTime(setupTime),
                    new PROperationalWindow(opWindow),
                    new PRCapacity(cap.ToString(), "Containers/Hour")
                );
            });
        }

        #endregion

        #region Status Tests

        [Fact]
        public void Should_Change_Status()
        {
            // Arrange
            var crane = new STSCrane(
                new PRCode("STS-010"),
                new PRDescription("Container Crane"),
                new PRArea("Dock 5"),
                new PRSetupTime(12),
                new PROperationalWindow("Mon-Fri 08:00-20:00"),
                new PRCapacity("40", "Containers/Hour")
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
        [InlineData("ST-001", "STS Crane", "Pier A", 30, 45)]
        [InlineData("ST-002", "Large STS Crane", "Pier B", 50, 65)]
        public void UpdateSTSCrane_ShouldChangeCapacity(
            string code, string desc, string area, double oldCap, double newCap)
        {
            // Arrange
            var crane = new STSCrane(
                new PRCode(code),
                new PRDescription(desc),
                new PRArea(area),
                new PRSetupTime(10),
                new PROperationalWindow("Mon-Fri 08:00-18:00"),
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
