using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Api.Controllers;
using PortManagement.Application.Common.Interfaces;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Domain.Vessel.Enums;
using Xunit;

namespace PortManagement.Tests.Api.Controllers
{
    public class VesselRecordControllerTests
    {
        private readonly Mock<IVesselRecordService> _serviceMock;
        private readonly Mock<IVesselTypeRepository> _vesselTypeRepoMock;
        private readonly VesselRecordController _controller;

        public VesselRecordControllerTests()
        {
            _serviceMock = new Mock<IVesselRecordService>();
            _vesselTypeRepoMock = new Mock<IVesselTypeRepository>();
            _controller = new VesselRecordController(_serviceMock.Object, _vesselTypeRepoMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithVesselRecords()
        {
            // Arrange
            var vesselType = CreateMockVesselType();
            var vessels = new List<VesselRecord>
            {
                new VesselRecord(
                    new Imo(1234567),
                    new VRName("Test Vessel"),
                    vesselType,
                    new VROwner("Test Owner"),
                    VRStatus.Active
                )
            };
            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(vessels);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDtos = Assert.IsAssignableFrom<IEnumerable<VesselRecordDto>>(okResult.Value);
            Assert.Single(returnedDtos);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenVesselExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vesselType = CreateMockVesselType();
            var vessel = new VesselRecord(
                new Imo(1234567),
                new VRName("Test Vessel"),
                vesselType,
                new VROwner("Test Owner"),
                VRStatus.Active
            );
            typeof(VesselRecord).GetProperty("Id")!.SetValue(vessel, id);
            _serviceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(vessel);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<VesselRecordDto>(okResult.Value);
            Assert.Equal(id, returnedDto.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenVesselDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((VesselRecord?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetByIMO_ReturnsOkResult_WhenVesselExists()
        {
            // Arrange
            int imoNumber = 1234567;
            var vesselType = CreateMockVesselType();
            var vessel = new VesselRecord(
                new Imo(1234567),
                new VRName("Test Vessel"),
                vesselType,
                new VROwner("Test Owner"),
                VRStatus.Active
            );

            _serviceMock.Setup(s => s.GetByIMOAsync(It.IsAny<Imo>(), It.IsAny<CancellationToken>())).ReturnsAsync(vessel);

            // Act
            var result = await _controller.GetByIMO(imoNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<VesselRecordDto>(okResult.Value);
            Assert.Equal("1234567", returnedDto.ImoValue);
        }

        [Fact]
        public async Task GetByIMO_ReturnsNotFound_WhenVesselDoesNotExist()
        {
            // Arrange
            int imoNumber = 1234567;
            _serviceMock.Setup(s => s.GetByIMOAsync(It.IsAny<Imo>(), It.IsAny<CancellationToken>())).ReturnsAsync((VesselRecord?)null);

            // Act
            var result = await _controller.GetByIMO(imoNumber);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            // Arrange
            var vesselType = CreateMockVesselType();
            var dto = new VesselRecordDto
            {
                ImoValue = "1234567",
                Name = "New Vessel",
                VesselTypeId = vesselType.Id,
                Owner = "New Owner",
                IsActive = true
            };

            _vesselTypeRepoMock.Setup(r => r.GetByIdAsync(dto.VesselTypeId, It.IsAny<CancellationToken>())).ReturnsAsync(vesselType);
            _serviceMock.Setup(s => s.AddAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(VesselRecordController.GetById), createdResult.ActionName);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new VesselRecordDto
            {
                Id = id,
                ImoValue = "1234567",
                Name = "Updated Vessel",
                VesselTypeId = Guid.NewGuid(),
                Owner = "Updated Owner",
                IsActive = true
            };

            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

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
            _serviceMock.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithMatchingVessels()
        {
            // Arrange
            string searchTerm = "Test";
            var vesselType = CreateMockVesselType();
            var vessels = new List<VesselRecord>
            {
                new VesselRecord(
                    new Imo(1234567),
                    new VRName("Test Vessel"),
                    vesselType,
                    new VROwner("Test Owner"),
                    VRStatus.Active
                )
            };

            _serviceMock.Setup(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>())).ReturnsAsync(vessels);

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDtos = Assert.IsAssignableFrom<IEnumerable<VesselRecordDto>>(okResult.Value);
            Assert.Single(returnedDtos);
        }

        [Fact]
        public async Task Inactivate_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.InactivateAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Inactivate(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        private VesselType CreateMockVesselType()
        {
            var vesselType = new VesselType
            {
                Name = new VTName("Container Ship"),
                Description = new VTDescription("Standard container vessel"),
                CapacityTEU = new VTCapacityTEU(5000),
                MaxRows = new VTMaxRows(10),
                MaxBays = new VTMaxBays(20),
                MaxTiers = new VTMaxTiers(8)
            };
            typeof(VesselType).GetProperty("Id")!.SetValue(vesselType, Guid.NewGuid());
            return vesselType;
        }
    }
}
