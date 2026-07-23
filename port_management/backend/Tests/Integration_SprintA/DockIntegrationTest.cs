using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Api.Controllers;
using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Docks.Entities;
using PortManagement.Domain.Docks.ValueObjects;
using PortManagement.Domain.PhysicalResources.Entities;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Tests.Integration_SprintA
{
    public class DockIntegrationTest
    {
        private readonly Mock<IDockService> _serviceMock;
        private readonly DockController _controller;
        private readonly DockMapper _mapper;
        private readonly Mock<IVesselTypeRepository> _vesselRepoMock;

        public DockIntegrationTest()
        {
            _vesselRepoMock = new Mock<IVesselTypeRepository>();
            _vesselRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<Guid> ids, CancellationToken ct) => ids.Select(id => new VesselType { Id = id }).ToList());
            _mapper = new DockMapper(_vesselRepoMock.Object);
            _serviceMock = new Mock<IDockService>();
            _controller = new DockController(_serviceMock.Object, _mapper);
        }

        #region US 2.2.3 - Create/Update Dock

        [Fact]
        public async Task CreateDock_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var dto = CreateValidDockDto();

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _serviceMock.Verify(s => s.AddAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDock_WithPhysicalCharacteristics_StoresCorrectly()
        {
            // Arrange - US 2.2.3: Dock must include length, depth, max draft
            var dto = CreateValidDockDto();
            dto.Length = 350;
            dto.Depth = 15;
            dto.MaxDraft = 14;

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreateDock_WithAllowedVesselTypes_SpecifiesRestrictions()
        {
            // Arrange - US 2.2.3: Officer must specify vessel types allowed
            var dto = CreateValidDockDto();
            dto.AllowedVesselTypes = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task UpdateDock_WithValidData_ReturnsOkResult()
        {
            // Arrange - US 2.2.3: Officers can update docks
            var dto = CreateValidDockDto();
            dto.Id = Guid.NewGuid();
            dto.Name = "Updated Dock A";

            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region US 2.2.3 - Search/Filter Docks

        [Fact]
        public async Task GetAllDocks_ReturnsAllDocks()
        {
            // Arrange - US 2.2.3: Docks must be searchable
            var docks = new List<Dock>
            {
                CreateMockDock("Dock A", "North Terminal"),
                CreateMockDock("Dock B", "South Terminal")
            };

            _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(docks);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<Dock>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task SearchDocks_ByName_ReturnsFilteredResults()
        {
            // Arrange - US 2.2.3: Searchable by name
            var docks = new List<Dock>
            {
                CreateMockDock("Container Dock 1", "West Pier"),
                CreateMockDock("Container Dock 2", "East Pier")
            };

            _serviceMock.Setup(s => s.SearchAsync("Container", null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(docks);

            // Act
            var result = await _controller.Search("Container", null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<Dock>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task SearchDocks_ByLocation_ReturnsFilteredResults()
        {
            // Arrange - US 2.2.3: Filterable by location
            var docks = new List<Dock>
            {
                CreateMockDock("Dock A", "North Terminal"),
                CreateMockDock("Dock B", "North Terminal")
            };

            _serviceMock.Setup(s => s.SearchAsync(null, "North", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(docks);

            // Act
            var result = await _controller.Search(null, "North", null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<Dock>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task SearchDocks_ByVesselType_ReturnsFilteredResults()
        {
            // Arrange - US 2.2.3: Filterable by vessel type
            var vesselTypeId = Guid.NewGuid();
            var docks = new List<Dock>
            {
                CreateMockDock("Container Dock", "Terminal 1")
            };

            _serviceMock.Setup(s => s.SearchAsync(null, null, vesselTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(docks);

            // Act
            var result = await _controller.Search(null, null, vesselTypeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<Dock>>(okResult.Value);
            Assert.Single(returnedList);
        }

        [Fact]
        public async Task GetDockById_WithValidId_ReturnsDock()
        {
            // Arrange
            var dockId = Guid.NewGuid();
            var dock = CreateMockDock("Dock A", "Terminal 1");

            _serviceMock.Setup(s => s.GetByIdAsync(dockId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dock);

            // Act
            var result = await _controller.GetById(dockId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDock = Assert.IsType<Dock>(okResult.Value);
            Assert.NotNull(returnedDock);
        }

        [Fact]
        public async Task GetDockById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var dockId = Guid.NewGuid();

            _serviceMock.Setup(s => s.GetByIdAsync(dockId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dock?)null);

            // Act
            var result = await _controller.GetById(dockId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Dock Deletion

        [Fact]
        public async Task DeleteDock_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var dockId = Guid.NewGuid();

            _serviceMock.Setup(s => s.DeleteAsync(dockId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(dockId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.DeleteAsync(dockId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Complete Workflow Tests

        [Fact]
        public async Task CompleteWorkflow_CreateUpdateDelete_SuccessfulFlow()
        {
            // Arrange - Complete workflow test
            var dto = CreateValidDockDto();

            // Step 1: Create
            _serviceMock.Setup(s => s.AddAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var createResult = await _controller.Create(dto);
            var createdOkResult = Assert.IsType<OkObjectResult>(createResult);
            Assert.NotNull(createdOkResult.Value);

            // Step 2: Update
            dto.Id = Guid.NewGuid();
            dto.Name = "Updated Dock";
            _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Dock>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var updateResult = await _controller.Update(dto);
            var updateOkResult = Assert.IsType<OkObjectResult>(updateResult);
            Assert.NotNull(updateOkResult.Value);

            // Step 3: Delete
            _serviceMock.Setup(s => s.DeleteAsync(dto.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var deleteResult = await _controller.Delete(dto.Id);
            Assert.IsType<NoContentResult>(deleteResult);
        }

        #endregion

        #region Helper Methods

        private DockDto CreateValidDockDto()
        {
            return new DockDto
            {
                Name = "Dock A",
                Location = "North Terminal",
                Length = 300,
                Depth = 12,
                MaxDraft = 11,
                MaxSTS = 4,
                AllowedVesselTypes = new List<Guid> { Guid.NewGuid() },
                STSCranes = new List<string>(),
                IsActive = true
            };
        }

        private Dock CreateMockDock(string name, string location)
        {
            return new Dock
            {
                Id = Guid.NewGuid(),
                Name = new DockName(name),
                Location = new DockLocation(location),
                Length = new DockLength(300),
                Depth = new DockDepth(12),
                MaxDraft = new DockMaxDraft(11),
                MaxSTS = new DockMaxSTS(4),
                AllowedVesselTypes = new List<VesselType>(),
                STSCranes = new List<STSCrane>(),
                IsActive = true
            };
        }

        #endregion
    }
}
