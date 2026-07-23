// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Moq;
// using PortManagement.Application.DTOs;
// using PortManagement.Application.Services.IServices;
// using PortManagement.Api.Controllers;
// using PortManagement.Domain.Vessel.Enums;
// using Xunit;

// namespace PortManagement.Tests.Api.Controllers
// {
//     public class VesselVisitNotificationControllerTests
//     {
//         private readonly Mock<IVesselVisitNotificationService> _serviceMock;
//         private readonly Mock<ILogger<VesselVisitNotificationController>> _loggerMock;
//         private readonly VesselVisitNotificationController _controller;

//         public VesselVisitNotificationControllerTests()
//         {
//             _serviceMock = new Mock<IVesselVisitNotificationService>();
//             _loggerMock = new Mock<ILogger<VesselVisitNotificationController>>();
//             _controller = new VesselVisitNotificationController(_serviceMock.Object, _loggerMock.Object);
//         }

//         [Fact]
//         public async Task GetAll_ReturnsOkResult_WithNotifications()
//         {
//             // Arrange
//             var notification = new VesselVisitNotificationDto
//             {
//                 BusinessId = "BID1",
//                 Status = VesselNotificationStatus.InProgress.ToString()
//             };
//             var notifications = new List<VesselVisitNotificationDto> { notification };
//             _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(notifications);

//             // Act
//             var result = await _controller.GetAll();

//             // Assert
//             var okResult = Assert.IsType<OkObjectResult>(result);
//             Assert.Equal(notifications, okResult.Value);
//         }

//         [Fact]
//         public async Task GetAll_ReturnsInternalServerError_OnException()
//         {
//             // Arrange
//             _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception());

//             // Act
//             var result = await _controller.GetAll();

//             // Assert
//             var statusResult = Assert.IsType<ObjectResult>(result);
//             Assert.Equal(500, statusResult.StatusCode);
//         }

//         [Fact]
//         public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
//         {
//             // Arrange
//             _controller.ModelState.AddModelError("BusinessId", "Required");
//             var dto = new VesselVisitNotificationDto();

//             // Act
//             var result = await _controller.Create(dto);

//             // Assert
//             var badRequest = Assert.IsType<BadRequestObjectResult>(result);
//         }


//     }
// }
