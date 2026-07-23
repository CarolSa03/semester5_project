using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Api.Controllers;
using Xunit;

namespace PortManagement.Tests.Controllers
{
    public class ShippingAgentRepresentativeControllerTests
    {
        private readonly Mock<IShippingAgentRepresentativeService> _serviceMock;
        private readonly ShippingAgentRepresentativeController _controller;

        public ShippingAgentRepresentativeControllerTests()
        {
            _serviceMock = new Mock<IShippingAgentRepresentativeService>();
            _controller = new ShippingAgentRepresentativeController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithRepresentatives()
        {
            // Arrange
            var reps = new List<ShippingAgentRepresentativeDto> { new ShippingAgentRepresentativeDto { Id = 1, Name = "Rep 1" } };
            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(reps);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reps, okResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("fail"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenRepresentativeExists()
        {
            // Arrange
            var rep = new ShippingAgentRepresentativeDto { Id = 1, Name = "Rep 1" };
            _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(rep);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(rep, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenRepresentativeDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((ShippingAgentRepresentativeDto?)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("fail"));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetByOrganization_ReturnsOkResult_WithRepresentatives()
        {
            // Arrange
            var reps = new List<ShippingAgentRepresentativeDto> { new ShippingAgentRepresentativeDto { Id = 1, Name = "Rep 1" } };
            _serviceMock.Setup(s => s.GetByOrganizationIdAsync("ORG1", It.IsAny<CancellationToken>())).ReturnsAsync(reps);

            // Act
            var result = await _controller.GetByOrganization("ORG1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reps, okResult.Value);
        }

        [Fact]
        public async Task GetByOrganization_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByOrganizationIdAsync("ORG1", It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("fail"));

            // Act
            var result = await _controller.GetByOrganization("ORG1");

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
