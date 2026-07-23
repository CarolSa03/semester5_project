using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.PhysicalResources.ValueObjects;
using PortManagement.Domain.PhysicalResources.Enums;
using Xunit;

namespace PortManagement.Tests.Domain.PhysicalResources
{
    public class TruckTests
    {
        #region Valid Construction

        [Theory]
        [InlineData("TRK-001", "Transport Truck", "Yard A", "10", "50")]
        [InlineData("TRK-002", "Cargo Truck", "Dock 2", "20", "45")]
        [InlineData("TRK-003", "Fuel Truck", "Yard B", "15", "60")]
        public void Should_Create_Truck_With_Valid_Data(
            string code, string description, string area,
            string capacity, string speed)
        {
            var truck = new Truck(
                new PRCode(code),
                new PRDescription(description),
                new PRArea(area),
                new PRSetupTime(10),
                new PROperationalWindow("24/7"),
                new PRCapacity(capacity, "Containers/Trip"),
                new PRSpeed(speed, "Km/h")
            );

            Assert.NotNull(truck);
            Assert.Equal(code, truck.Code.Value);
            Assert.Equal(description, truck.Description.Value);
            Assert.Equal(area, truck.Area.Value);
            Assert.True(double.Parse(capacity) > 0);
            Assert.Equal("Containers/Trip", truck.Capacity.Unit);
            Assert.True(double.Parse(speed) > 0);
            Assert.Equal("Km/h", truck.Speed.Unit);
            Assert.Equal(PRStatus.Active, truck.Status);
        }

        #endregion

        #region Invalid Construction

        [Theory]
        [InlineData("", "Truck Desc", "Yard", "10", "50")]
        [InlineData("TRK-001", "", "Yard", "10", "50")]
        [InlineData("TRK-001", "Truck Desc", "", "10", "50")]
        [InlineData("TRK-001", "Truck Desc", "Yard", "-5", "50")]
        public void Should_Throw_Exception_For_Invalid_Data(
            string code, string description, string area,
            string capacity, string speed)
        {
            Assert.ThrowsAny<ArgumentException>(() =>
                new Truck(
                    new PRCode(code),
                    new PRDescription(description),
                    new PRArea(area),
                    new PRSetupTime(10),
                    new PROperationalWindow("24/7"),
                    new PRCapacity(capacity, "Containers/Trip"),
                    new PRSpeed(speed, "Km/h")
                )
            );
        }

        #endregion

        #region Status Tests

        [Fact]
        public void Should_Change_Status()
        {
            var truck = new Truck(
                new PRCode("TRK-010"),
                new PRDescription("Container Truck"),
                new PRArea("Dock 5"),
                new PRSetupTime(15),
                new PROperationalWindow("24/7"),
                new PRCapacity("12", "Containers/Trip"),
                new PRSpeed("45", "Km/h")
            );

            truck.MarkInactive();
            Assert.Equal(PRStatus.Inactive, truck.Status);

            truck.MarkMaintenance();
            Assert.Equal(PRStatus.Maintenance, truck.Status);

            truck.MarkActive();
            Assert.Equal(PRStatus.Active, truck.Status);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void UpdateTruck_ShouldUpdateCapacityAndSpeed()
        {
            // Arrange
            var truck = new Truck(
                new PRCode("TRK-001"),
                new PRDescription("Truck"),
                new PRArea("Dock A"),
                new PRSetupTime(10),
                new PROperationalWindow("24/7"),
                new PRCapacity("10", "Containers/Trip"),
                new PRSpeed("40", "Km/h")
            );

            var newCapacity = new PRCapacity("20", "Containers/Trip");
            var newSpeed = new PRSpeed("60", "Km/h");

            // Act
            truck.UpdateCapacity(newCapacity);
            truck.UpdateSpeed(newSpeed);

            // Assert
            Assert.Equal(20, truck.Capacity.Value);
            Assert.Equal(60, truck.Speed.Value);
        }

        [Fact]
        public void UpdateTruck_ShouldUpdateBaseFields()
        {
            // Arrange
            var truck = new Truck(
                new PRCode("TRK-001"),
                new PRDescription("Truck"),
                new PRArea("Dock A"),
                new PRSetupTime(10),
                new PROperationalWindow("24/7"),
                new PRCapacity("10", "Containers/Trip"),
                new PRSpeed("40", "Km/h")
            );

            var newDescription = new PRDescription("Updated Truck");
            var newArea = new PRArea("Dock B");

            // Act
            truck.UpdateBaseFields(description: newDescription, area: newArea, status: PRStatus.Maintenance);

            // Assert
            Assert.Equal("Updated Truck", truck.Description.Value);
            Assert.Equal("Dock B", truck.Area.Value);
            Assert.Equal(PRStatus.Maintenance, truck.Status);
        }

        #endregion
    }
}
