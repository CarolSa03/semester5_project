using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Api.Controllers;
using PortManagement.Application.Mappers;
using PortManagement.Domain.Docks.Entities;
using Xunit;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Tests.Api.Controllers;

public class DockControllerTests
{
    private readonly Mock<IDockService> _serviceMock;
    private readonly DockController _controller;
    private readonly DockMapper _mapper;
    private readonly Mock<IVesselTypeRepository> _vesselRepoMock;

    public DockControllerTests()
    {
        _vesselRepoMock = new Mock<IVesselTypeRepository>();
        // Default behavior: when mapper asks for vessel types by ids, return a list with matching ids
        _vesselRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Guid> ids, CancellationToken ct) => ids.Select(id => new PortManagement.Domain.Vessel.Entities.VesselType { Id = id }).ToList());
        _mapper = new DockMapper(_vesselRepoMock.Object);
        _serviceMock = new Mock<IDockService>();
        _controller = new DockController(_serviceMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithDocks()
    {
        // Arrange
        var docks = new List<Dock> { new Dock { Id = Guid.NewGuid() } };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(docks);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(docks, okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_OnException()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenDockExists()
    {
        // Arrange
        var dockId = Guid.NewGuid();
        var dock = new Dock { Id = dockId };
        _serviceMock.Setup(s => s.GetByIdAsync(dockId, It.IsAny<CancellationToken>())).ReturnsAsync(dock);

        // Act
        var result = await _controller.GetById(dockId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dock, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDockDoesNotExist()
    {
        // Arrange
        var dockId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(dockId, It.IsAny<CancellationToken>())).ReturnsAsync((Dock?)null);

        // Act
        var result = await _controller.GetById(dockId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsInternalServerError_OnException()
    {
        // Arrange
        var dockId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(dockId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetById(dockId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
