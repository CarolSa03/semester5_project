using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Api.Controllers;
using Xunit;

namespace PortManagement.Tests.Api.Controllers
{
    public class VesselTypeControllerTests
    {
        private readonly Mock<IVesselTypeService> _serviceMock;
        private readonly VesselTypeController _controller;

        public VesselTypeControllerTests()
        {
            _serviceMock = new Mock<IVesselTypeService>();
            _controller = new VesselTypeController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithVesselTypes()
        {
            // Arrange
            var id1 = Guid.NewGuid();
            var types = new List<VesselTypeDto> { new VesselTypeDto { Id = id1, Name = "Type 1" } };
            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(types);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(types, okResult.Value);
        }

        [Fact]
        public async Task GetActive_ReturnsOkResult_WithActiveVesselTypes()
        {
            // Arrange
            var id2 = Guid.NewGuid();
            var types = new List<VesselTypeDto> { new VesselTypeDto { Id = id2, Name = "Active Type" } };
            _serviceMock.Setup(s => s.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(types);

            // Act
            var result = await _controller.GetActive();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(types, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenVesselTypeExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var type = new VesselTypeDto { Id = id, Name = "Type 1" };
            _serviceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(type);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(type, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenVesselTypeDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((VesselTypeDto)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

