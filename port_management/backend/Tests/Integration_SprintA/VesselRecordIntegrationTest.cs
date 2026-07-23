using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Api.Controllers;
using PortManagement.Application.Common.Interfaces;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Tests.Integration_SprintA
{
    public class VesselRecordIntegrationTest
    {
        private readonly Mock<IVesselRecordService> _serviceMock;
        private readonly Mock<IVesselTypeRepository> _vesselTypeRepoMock;
        private readonly VesselRecordController _controller;

        public VesselRecordIntegrationTest()
        {
            _serviceMock = new Mock<IVesselRecordService>();
            _vesselTypeRepoMock = new Mock<IVesselTypeRepository>();
            _controller = new VesselRecordController(_serviceMock.Object, _vesselTypeRepoMock.Object);
        }

        #region US 2.2.2 - Create/Update Vessel Record

        [Fact]
        public async Task CreateVesselRecord_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var dto = CreateValidVesselRecordDto();
            var vesselType = CreateMockVesselType();

            _vesselTypeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vesselType);

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedDto = Assert.IsType<VesselRecordDto>(createdResult.Value);

            Assert.Equal("GetById", createdResult.ActionName);
            Assert.NotNull(returnedDto.Id);
            Assert.Equal(dto.ImoValue, returnedDto.ImoValue);
            Assert.Equal(dto.Name, returnedDto.Name);
            _serviceMock.Verify(s => s.AddAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateVesselRecord_WithInvalidIMO_ReturnsBadRequest()
        {
            // Arrange - US 2.2.2: IMO must follow official format (7 digits + check digit)
            var dto = CreateValidVesselRecordDto();
            dto.ImoValue = "INVALID123"; // Invalid IMO format

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateVesselRecord_WithInactiveVesselType_ReturnsBadRequest()
        {
            // Arrange - US 2.2.2: Vessel type must be valid and active
            var dto = CreateValidVesselRecordDto();
            var inactiveVesselType = CreateMockVesselType();
            inactiveVesselType.IsActive = false;

            _vesselTypeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inactiveVesselType);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateVesselRecord_WithNonExistentVesselType_ReturnsBadRequest()
        {
            // Arrange
            var dto = CreateValidVesselRecordDto();

            _vesselTypeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VesselType?)null);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateVesselRecord_WithValidData_ReturnsNoContent()
        {
            // Arrange - US 2.2.2: Officers can update vessel records
            var vesselId = Guid.NewGuid();
            var dto = CreateValidVesselRecordDto();
            dto.Id = vesselId;
            dto.Name = "Updated Vessel Name";

            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(vesselId, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateVesselRecord_NotFound_ReturnsNotFound()
        {
            // Arrange
            var vesselId = Guid.NewGuid();
            var dto = CreateValidVesselRecordDto();
            dto.Id = vesselId;

            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException("Vessel record not found"));

            // Act
            var result = await _controller.Update(vesselId, dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region US 2.2.2 - Search/Filter Vessel Records

        [Fact]
        public async Task GetAllVesselRecords_ReturnsAllRecords()
        {
            // Arrange - US 2.2.2: Vessel records must be searchable
            var vessels = new List<VesselRecord>
            {
                CreateMockVesselRecord("9074729", "MSC OSCAR"),
                CreateMockVesselRecord("9321483", "EMMA MAERSK")
            };

            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(vessels);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<VesselRecordDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task GetVesselRecordByIMO_WithValidIMO_ReturnsVessel()
        {
            // Arrange - US 2.2.2: Searchable by IMO number
            var imoNumber = 9074729;
            var vessel = CreateMockVesselRecord(imoNumber.ToString(), "MSC OSCAR");

            _serviceMock.Setup(s => s.GetByIMOAsync(It.IsAny<Imo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vessel);

            // Act
            var result = await _controller.GetByIMO(imoNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<VesselRecordDto>(okResult.Value);
            Assert.Equal(imoNumber.ToString(), returnedDto.ImoValue);
            Assert.Equal("MSC OSCAR", returnedDto.Name);
        }

        [Fact]
        public async Task GetVesselRecordByIMO_WithInvalidIMO_ReturnsBadRequest()
        {
            // Arrange
            var invalidIMO = 123; // Invalid IMO (doesn't pass validation)

            // Act
            var result = await _controller.GetByIMO(invalidIMO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetVesselRecordByIMO_NotFound_ReturnsNotFound()
        {
            // Arrange
            var imoNumber = 9074729;

            _serviceMock.Setup(s => s.GetByIMOAsync(It.IsAny<Imo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VesselRecord?)null);

            // Act
            var result = await _controller.GetByIMO(imoNumber);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task SearchVesselRecords_ByName_ReturnsFilteredResults()
        {
            // Arrange - US 2.2.2: Searchable by name
            var searchTerm = "MAERSK";
            var vessels = new List<VesselRecord>
            {
                CreateMockVesselRecord("9321483", "EMMA MAERSK"),
                CreateMockVesselRecord("9074729", "MAERSK EDINBURGH")
            };

            _serviceMock.Setup(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vessels);

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<VesselRecordDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
            Assert.All(returnedList, dto => Assert.Contains("MAERSK", dto.Name));
        }

        [Fact]
        public async Task SearchVesselRecords_ByOperator_ReturnsFilteredResults()
        {
            // Arrange - US 2.2.2: Searchable by operator
            var searchTerm = "MSC";
            var vessels = new List<VesselRecord>
            {
                CreateMockVesselRecord("9074729", "MSC OSCAR", "MSC Mediterranean Shipping Company")
            };

            _serviceMock.Setup(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vessels);

            // Act
            var result = await _controller.Search(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<VesselRecordDto>>(okResult.Value);
            Assert.Single(returnedList);
        }

        [Fact]
        public async Task GetVesselRecordById_WithValidId_ReturnsVessel()
        {
            // Arrange
            var vesselId = Guid.NewGuid();
            var vessel = CreateMockVesselRecord("9074729", "MSC OSCAR");

            _serviceMock.Setup(s => s.GetByIdAsync(vesselId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vessel);

            // Act
            var result = await _controller.GetById(vesselId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<VesselRecordDto>(okResult.Value);
            Assert.NotNull(returnedDto);
        }

        [Fact]
        public async Task GetVesselRecordById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var vesselId = Guid.NewGuid();

            _serviceMock.Setup(s => s.GetByIdAsync(vesselId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VesselRecord?)null);

            // Act
            var result = await _controller.GetById(vesselId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region Vessel Inactivation/Deletion

        [Fact]
        public async Task InactivateVesselRecord_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var vesselId = Guid.NewGuid();

            _serviceMock.Setup(s => s.InactivateAsync(vesselId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Inactivate(vesselId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.InactivateAsync(vesselId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InactivateVesselRecord_NotFound_ReturnsNotFound()
        {
            // Arrange
            var vesselId = Guid.NewGuid();

            _serviceMock.Setup(s => s.InactivateAsync(vesselId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException("Vessel record not found"));

            // Act
            var result = await _controller.Inactivate(vesselId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteVesselRecord_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var vesselId = Guid.NewGuid();

            _serviceMock.Setup(s => s.DeleteAsync(vesselId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(vesselId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.DeleteAsync(vesselId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Complete Workflow Tests

        [Fact]
        public async Task CompleteWorkflow_CreateUpdateInactivate_SuccessfulFlow()
        {
            // Arrange - Complete workflow test
            var vesselType = CreateMockVesselType();

            // Step 1: Create
            var createDto = CreateValidVesselRecordDto();

            _vesselTypeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vesselType);

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act & Assert - Create
            var createResult = await _controller.Create(createDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(createResult);
            var createdDto = Assert.IsType<VesselRecordDto>(createdResult.Value);
            var vesselId = createdDto.Id!.Value;

            // Step 2: Update
            createdDto.Name = "Updated Vessel Name";
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<VesselRecord>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var updateResult = await _controller.Update(vesselId, createdDto);
            Assert.IsType<NoContentResult>(updateResult);

            // Step 3: Inactivate
            _serviceMock.Setup(s => s.InactivateAsync(vesselId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var inactivateResult = await _controller.Inactivate(vesselId);
            Assert.IsType<NoContentResult>(inactivateResult);
        }

        #endregion

        #region Helper Methods

        private VesselRecordDto CreateValidVesselRecordDto()
        {
            return new VesselRecordDto
            {
                ImoValue = "9074729",
                Name = "MSC OSCAR",
                VesselTypeId = Guid.NewGuid(),
                Owner = "MSC Mediterranean Shipping Company",
                IsActive = true
            };
        }

        private VesselType CreateMockVesselType()
        {
            return new VesselType
            {
                Id = Guid.NewGuid(),
                Name = new VTName("Container Ship"),
                Description = new VTDescription("Large container vessel"),
                CapacityTEU = new VTCapacityTEU(19224),
                MaxBays = new VTMaxBays(23),
                MaxRows = new VTMaxRows(24),
                MaxTiers = new VTMaxTiers(10),
                IsActive = true
            };
        }

        private VesselRecord CreateMockVesselRecord(string imoValue, string name, string owner = "Test Owner")
        {
            var imo = Imo.TryCreate(int.Parse(imoValue), out var imoVo) ? imoVo : throw new ArgumentException("Invalid IMO");
            var vesselType = CreateMockVesselType();

            return new VesselRecord(
                imo,
                new VRName(name),
                vesselType,
                new VROwner(owner),
                VRStatus.Active
            );
        }

        #endregion
    }
}
