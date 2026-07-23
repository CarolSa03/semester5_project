using Microsoft.AspNetCore.Mvc;
using Moq;
using PortManagement.Api.Controllers;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Qualification.Entities;

namespace PortManagement.Tests.Integration_SprintA
{
    public class QualificationIntegrationTest
    {
        private readonly Mock<IQualificationService> _serviceMock;
        private readonly QualificationController _controller;

        public QualificationIntegrationTest()
        {
            _serviceMock = new Mock<IQualificationService>();
            _controller = new QualificationController(_serviceMock.Object);
        }

        #region US 2.2.13 - Create/Update Qualification

        [Fact]
        public async Task CreateQualification_WithValidData_ReturnsCreatedResult()
        {
            // Arrange - US 2.2.13: Create qualifications
            var dto = CreateValidQualificationDto();

            _serviceMock.Setup(s => s.CreateQualificationAsync(It.IsAny<Qualification>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            _serviceMock.Verify(s => s.CreateQualificationAsync(It.IsAny<Qualification>()), Times.Once);
        }

        [Fact]
        public async Task UpdateQualification_WithValidData_ReturnsOkResult()
        {
            // Arrange - US 2.2.13: Update qualifications
            var qualificationId = Guid.NewGuid();
            var dto = CreateValidQualificationDto();
            dto.Id = qualificationId;
            dto.Description = "Updated Description";

            _serviceMock.Setup(s => s.UpdateQualificationAsync(It.IsAny<Qualification>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(qualificationId, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.UpdateQualificationAsync(It.IsAny<Qualification>()), Times.Once);
        }

        #endregion

        #region US 2.2.13 - Search/Filter Qualifications

        [Fact]
        public async Task GetAllQualifications_ReturnsAllQualifications()
        {
            // Arrange - US 2.2.13: Qualifications must be searchable
            var qualifications = new List<Qualification>
            {
                CreateMockQualification("CRN-001", "Crane Operator"),
                CreateMockQualification("TRK-001", "Truck Driver")
            };

            _serviceMock.Setup(s => s.GetAllQualificationsAsync())
                .ReturnsAsync(qualifications);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<QualificationDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }

        [Fact]
        public async Task GetQualificationByCode_WithValidCode_ReturnsQualification()
        {
            // Arrange - US 2.2.13: Searchable by code
            var qualification = CreateMockQualification("CRN-001", "Crane Operator");

            _serviceMock.Setup(s => s.GetQualificationByCodeAsync("CRN-001"))
                .ReturnsAsync(qualification);

            // Act
            var result = await _controller.GetById("CRN-001");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<QualificationDto>(okResult.Value);
            Assert.Equal("CRN-001", returnedDto.Code);
        }

        [Fact]
        public async Task GetQualificationByCode_NotFound_ReturnsNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetQualificationByCodeAsync("INVALID"))
                .ReturnsAsync((Qualification?)null);

            // Act
            var result = await _controller.GetById("INVALID");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetQualificationByName_WithValidName_ReturnsQualification()
        {
            // Arrange - US 2.2.13: Filterable by name
            var qualification = CreateMockQualification("CRN-001", "Crane Operator");

            _serviceMock.Setup(s => s.GetQualificationByNameAsync("Crane Operator"))
                .ReturnsAsync(qualification);

            // Act
            var result = await _controller.GetByDescriptiveName("Crane Operator");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<QualificationDto>(okResult.Value);
            Assert.Equal("Crane Operator", returnedDto.Description);
        }

        #endregion

        #region Complete Workflow Tests

        [Fact]
        public async Task CompleteWorkflow_CreateUpdateSearch_SuccessfulFlow()
        {
            // Arrange
            var dto = CreateValidQualificationDto();

            // Step 1: Create
            _serviceMock.Setup(s => s.CreateQualificationAsync(It.IsAny<Qualification>()))
                .Returns(Task.CompletedTask);

            var createResult = await _controller.Create(dto);
            Assert.IsType<CreatedAtActionResult>(createResult);

            // Step 2: Update
            dto.Id = Guid.NewGuid();
            dto.Description = "Updated Qualification";
            _serviceMock.Setup(s => s.UpdateQualificationAsync(It.IsAny<Qualification>()))
                .Returns(Task.CompletedTask);

            var updateResult = await _controller.Update(dto.Id.Value, dto);
            Assert.IsType<NoContentResult>(updateResult);

            // Step 3: Search
            var qualification = CreateMockQualification(dto.Code ?? "Q001", dto.Description ?? "Test");
            _serviceMock.Setup(s => s.GetQualificationByCodeAsync(dto.Code ?? "Q001"))
                .ReturnsAsync(qualification);

            var searchResult = await _controller.GetById(dto.Code ?? "Q001");
            var okResult = Assert.IsType<OkObjectResult>(searchResult.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region Helper Methods

        private QualificationDto CreateValidQualificationDto()
        {
            return new QualificationDto
            {
                Code = "CRN-001",
                Description = "STS Crane Operator"
            };
        }

        private Qualification CreateMockQualification(string code, string description)
        {
            return new Qualification(code, description)
            {
                Id = Guid.NewGuid()
            };
        }

        #endregion
    }
}
