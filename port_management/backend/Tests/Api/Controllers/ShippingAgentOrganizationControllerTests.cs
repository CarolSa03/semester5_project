using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Api.Controllers;
using Xunit;

namespace PortManagement.Tests.Api.Controllers;

public class ShippingAgentOrganizationControllerTests
{
    private readonly Mock<IShippingAgentOrganizationService> _serviceMock;
    private readonly ShippingAgentOrganizationController _controller;

    public ShippingAgentOrganizationControllerTests()
    {
        _serviceMock = new Mock<IShippingAgentOrganizationService>();
        _controller = new ShippingAgentOrganizationController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithOrganizations()
    {
        // Arrange
        var orgs = new List<ShippingAgentOrganizationDto> { new ShippingAgentOrganizationDto { Id = "ORG1", LegalName = "Org 1" } };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(orgs);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orgs, okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_OnException()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WhenOrganizationExists()
    {
        // Arrange
        var org = new ShippingAgentOrganizationDto { Id = "ORG1", LegalName = "Org 1" };
        _serviceMock.Setup(s => s.GetByIdAsync("ORG1", It.IsAny<CancellationToken>())).ReturnsAsync(org);

        // Act
        var result = await _controller.GetById("ORG1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(org, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenOrganizationDoesNotExist()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync("ORG1", It.IsAny<CancellationToken>())).ReturnsAsync((ShippingAgentOrganizationDto?)null);

        // Act
        var result = await _controller.GetById("ORG1");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsInternalServerError_OnException()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync("ORG1", It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));

        // Act
        var result = await _controller.GetById("ORG1");

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
