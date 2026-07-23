using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Api.Controllers;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;

namespace PortManagement.Tests.Integration_SprintA
{
    public class VesselTypeIntegrationTest
    {
        private readonly Mock<IVesselTypeService> _serviceMock;
        private readonly VesselTypeController _controller;

        public VesselTypeIntegrationTest()
        {
            _serviceMock = new Mock<IVesselTypeService>();
            _controller = new VesselTypeController(_serviceMock.Object);
        }

        #region US 2.2.1 - Create/Update Vessel Type

        [Fact]
        public async Task CreateVesselType_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var dto = CreateValidVesselTypeDto();
            var createdDto = CreateValidVesselTypeDto();
            createdDto.Id = Guid.NewGuid();

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedDto = Assert.IsType<VesselTypeDto>(createdResult.Value);
            Assert.Equal("GetById", createdResult.ActionName);
            Assert.NotEqual(Guid.Empty, returnedDto.Id);
            _serviceMock.Verify(s => s.AddAsync(It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateVesselType_WithOperationalConstraints_StoresCorrectly()
        {
            // Arrange - US 2.2.1: Include capacity and operational constraints
            var dto = CreateValidVesselTypeDto();
            dto.CapacityTEU = 18000;
            dto.MaxRows = 24;
            dto.MaxBays = 22;
            dto.MaxTiers = 10;

            var createdDto = CreateValidVesselTypeDto();
            createdDto.Id = Guid.NewGuid();

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task UpdateVesselType_WithValidData_ReturnsUpdatedDto()
        {
            // Arrange - US 2.2.1: Officers can update vessel types
            var vesselTypeId = Guid.NewGuid();
            var dto = CreateValidVesselTypeDto();
            dto.Name = "Updated Container Ship";

            var updatedDto = CreateValidVesselTypeDto();
            updatedDto.Id = vesselTypeId;
            updatedDto.Name = "Updated Container Ship";

            _serviceMock.Setup(s => s.UpdateAsync(vesselTypeId, It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedDto);

            // Act
            var result = await _controller.Update(vesselTypeId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<VesselTypeDto>(okResult.Value);
            Assert.Equal("Updated Container Ship", returnedDto.Name);
            _serviceMock.Verify(s => s.UpdateAsync(vesselTypeId, It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateVesselType_NotFound_ReturnsNotFound()
        {
            // Arrange
            var vesselTypeId = Guid.NewGuid();
            var dto = CreateValidVesselTypeDto();

            _serviceMock.Setup(s => s.UpdateAsync(vesselTypeId, It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VesselTypeDto?)null);

            // Act
            var result = await _controller.Update(vesselTypeId, dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region US 2.2.1 - Search/Filter Vessel Types

        [Fact]
        public async Task GetAllVesselTypes_ReturnsAllTypes()
        {
            // Arrange - US 2.2.1: Vessel types must be searchable
            var vesselTypes = new List<VesselTypeDto>
            {
                CreateValidVesselTypeDto(),
                CreateValidVesselTypeDto()
            };

            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(vesselTypes);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<VesselTypeDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task GetActiveVesselTypes_ReturnsOnlyActive()
        {
            // Arrange
            var activeVesselTypes = new List<VesselTypeDto>
            {
                CreateValidVesselTypeDto(),
                CreateValidVesselTypeDto()
            };

            _serviceMock.Setup(s => s.GetActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeVesselTypes);

            // Act
            var result = await _controller.GetActive();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<VesselTypeDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task GetVesselTypeById_WithValidId_ReturnsType()
        {
            // Arrange
            var vesselTypeId = Guid.NewGuid();
            var vesselTypeDto = CreateValidVesselTypeDto();
            vesselTypeDto.Id = vesselTypeId;

            _serviceMock.Setup(s => s.GetByIdAsync(vesselTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vesselTypeDto);

            // Act
            var result = await _controller.GetById(vesselTypeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedType = Assert.IsType<VesselTypeDto>(okResult.Value);
            Assert.NotNull(returnedType);
        }

        [Fact]
        public async Task GetVesselTypeById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var vesselTypeId = Guid.NewGuid();

            _serviceMock.Setup(s => s.GetByIdAsync(vesselTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VesselTypeDto?)null);

            // Act
            var result = await _controller.GetById(vesselTypeId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Vessel Type Activation/Deactivation

        [Fact]
        public async Task InactivateVesselType_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var vesselTypeId = Guid.NewGuid();
            var inactivatedDto = CreateValidVesselTypeDto();
            inactivatedDto.Id = vesselTypeId;
            inactivatedDto.IsActive = false;

            _serviceMock.Setup(s => s.InactivateAsync(vesselTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(inactivatedDto);

            // Act
            var result = await _controller.Inactivate(vesselTypeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<VesselTypeDto>(okResult.Value);
            Assert.False(returnedDto.IsActive);
        }

        #endregion

        #region Complete Workflow Tests

        [Fact]
        public async Task CompleteWorkflow_CreateUpdateInactivate_SuccessfulFlow()
        {
            // Arrange - Complete workflow test
            var dto = CreateValidVesselTypeDto();
            var createdDto = CreateValidVesselTypeDto();
            createdDto.Id = Guid.NewGuid();

            // Step 1: Create
            _serviceMock.Setup(s => s.AddAsync(It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDto);

            var createResult = await _controller.Create(dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(createResult);
            var vesselTypeDto = Assert.IsType<VesselTypeDto>(createdResult.Value);
            var vesselTypeId = vesselTypeDto.Id;

            // Step 2: Update
            vesselTypeDto.Name = "Updated Vessel Type";
            _serviceMock.Setup(s => s.UpdateAsync(vesselTypeId, It.IsAny<VesselTypeDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vesselTypeDto);

            var updateResult = await _controller.Update(vesselTypeId, vesselTypeDto);
            Assert.IsType<OkObjectResult>(updateResult);

            // Step 3: Inactivate
            vesselTypeDto.IsActive = false;
            _serviceMock.Setup(s => s.InactivateAsync(vesselTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vesselTypeDto);

            var inactivateResult = await _controller.Inactivate(vesselTypeId);
            var inactivatedOkResult = Assert.IsType<OkObjectResult>(inactivateResult);
            var inactivatedDto = Assert.IsType<VesselTypeDto>(inactivatedOkResult.Value);
            Assert.False(inactivatedDto.IsActive);
        }

        #endregion

        #region Helper Methods

        private VesselTypeDto CreateValidVesselTypeDto()
        {
            return new VesselTypeDto
            {
                Name = "Container Ship",
                Description = "Large container vessel",
                CapacityTEU = 18000,
                MaxRows = 24,
                MaxBays = 22,
                MaxTiers = 10,
                IsActive = true
            };
        }

        #endregion
    }
}
