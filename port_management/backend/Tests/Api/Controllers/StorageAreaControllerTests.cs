using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Api.Controllers;
using PortManagement.Domain.Storage.Entities;
using PortManagement.Domain.Storage.ValueObjects;
using PortManagement.Domain.Storage.Enums;
using Xunit;

namespace PortManagement.Tests.Api.Controllers
{
    public class StorageAreaControllerTests
    {
        private readonly Mock<IStorageAreaService> _serviceMock;
        private readonly StorageAreaController _controller;

        public StorageAreaControllerTests()
        {
            _serviceMock = new Mock<IStorageAreaService>();
            _controller = new StorageAreaController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithStorageAreas()
        {
            // Arrange
            var storageAreas = new List<StorageArea>
            {
                new StorageArea(
                    new SAId("SA-001"),
                    SAType.Yard,
                    new SALocation("North Zone"),
                    new SACapacity(1000),  // maxCapacity
                    new SACapacity(500),   // currentCapacity
                    new SANotes("Test notes")
                )
            };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(storageAreas);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDtos = Assert.IsAssignableFrom<IEnumerable<StorageAreaDto>>(okResult.Value);
            Assert.Single(returnedDtos);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenStorageAreaExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var storageArea = new StorageArea(
                new SAId("SA-001"),
                SAType.Warehouse,
                new SALocation("South Zone"),
                new SACapacity(2000),  // maxCapacity
                new SACapacity(1000),  // currentCapacity
                new SANotes("Test warehouse")
            );
            typeof(StorageArea).GetProperty("Id")!.SetValue(storageArea, id);

            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(storageArea);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<StorageAreaDto>(okResult.Value);
            Assert.Equal(id, returnedDto.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenStorageAreaDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((StorageArea?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            // Arrange
            var dto = new StorageAreaDto
            {
                BusinessId = "SA-NEW",
                Type = "Yard",
                Location = "East Zone",
                MaxCapacityTEU = 1500,
                CurrentCapacityTEU = 0
            };

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<StorageArea>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(StorageAreaController.GetById), createdResult.ActionName);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new StorageAreaDto
            {
                Id = id,
                BusinessId = "SA-001",
                Type = "Yard",
                Location = "Updated Location",
                MaxCapacityTEU = 1500,
                CurrentCapacityTEU = 750
            };

            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<StorageArea>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetByBusinessId_ReturnsOkResult_WhenStorageAreaExists()
        {
            // Arrange
            var businessId = "SA-TEST";
            var storageArea = new StorageArea(
                new SAId(businessId),
                SAType.Yard,
                new SALocation("Test Location"),
                new SACapacity(1000),  // maxCapacity
                new SACapacity(500),   // currentCapacity
                new SANotes("Test")
            );

            _serviceMock.Setup(s => s.GetByBusinessIdAsync(businessId)).ReturnsAsync(storageArea);

            // Act
            var result = await _controller.GetByBusinessId(businessId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<StorageAreaDto>(okResult.Value);
            Assert.Equal(businessId, returnedDto.BusinessId);
        }

        [Fact]
        public async Task GetByBusinessId_ReturnsNotFound_WhenStorageAreaDoesNotExist()
        {
            // Arrange
            var businessId = "SA-NOTFOUND";
            _serviceMock.Setup(s => s.GetByBusinessIdAsync(businessId)).ReturnsAsync((StorageArea?)null);

            // Act
            var result = await _controller.GetByBusinessId(businessId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}

