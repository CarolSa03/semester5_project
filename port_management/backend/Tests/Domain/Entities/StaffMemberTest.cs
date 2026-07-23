using FluentAssertions;
using PortManagement.Domain.Staff.Entities;
using PortManagement.Domain.Staff.ValueObjects;
using PortManagement.Domain.PhysicalResources.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.Entities
{
    public class StaffMemberTest
    {
        #region StaffMember Creation

        [Fact]
        public void CreateStaffMember_WithValidProperties_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var smId = new SMId("ST-001");
            var name = new SMName("João Silva");
            var email = new Email("joao.silva@porto.com");
            var phone = new PhoneNumber("912345678");
            var window = new SMOperationalWindow("Mon-Fri 08:00-17:00");
            var qualifications = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            // Act
            var staff = new StaffMember(smId, name, email, phone, window, qualifications);

            // Assert
            staff.Id.Should().NotBe(Guid.Empty);
            staff.StaffMemberId.Value.Should().Be("ST-001");
            staff.ShortName.Value.Should().Be("João Silva");
            staff.Email.Value.Should().Be("joao.silva@porto.com");
            staff.PhoneNumber.Value.Should().Be("912345678");
            staff.OperationalWindow.Value.Should().Be("Mon-Fri 08:00-17:00");
            staff.IsAvailable.Should().BeTrue();
            staff.QualificationIds.Should().BeEquivalentTo(qualifications);
        }

        [Fact]
        public void CreateStaffMember_WithNullQualifications_ShouldHaveEmptyQualificationList()
        {
            // Arrange
            var smId = new SMId("ST-002");
            var name = new SMName("Maria Santos");
            var email = new Email("maria@porto.com");
            var phone = new PhoneNumber("900000000");
            var window = new SMOperationalWindow("24/7");

            // Act
            var staff = new StaffMember(smId, name, email, phone, window);

            // Assert
            staff.QualificationIds.Should().BeEmpty();
        }

        [Fact]
        public void CreateStaffMember_WithNullShortName_ShouldThrowException()
        {
            // Arrange
            var smId = new SMId("ST-003");
            SMName? shortName = null;
            var email = new Email("test@porto.com");
            var phone = new PhoneNumber("933221100");
            var window = new SMOperationalWindow("Mon-Fri 09:00-18:00");

            // Act
            Action act = () =>
                new StaffMember(smId, shortName!, email, phone, window);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("shortName");
        }

        [Fact]
        public void CreateStaffMember_WithNullOperationalWindow_ShouldThrowException()
        {
            // Arrange
            var smId = new SMId("ST-004");
            var name = new SMName("Luis");
            var email = new Email("luis@porto.com");
            var phone = new PhoneNumber("955000111");
            SMOperationalWindow? window = null;

            // Act
            Action act = () =>
                new StaffMember(smId, name, email, phone, window!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("operationalWindow");
        }

        #endregion

        #region Availability

        [Fact]
        public void SetAvailability_ShouldChangeAvailabilityState()
        {
            // Arrange
            var staff = new StaffMember(
                new SMId("ST-005"),
                new SMName("Pedro"),
                new Email("pedro@porto.com"),
                new PhoneNumber("977111222"),
                new SMOperationalWindow("24/7")
            );

            // Act
            staff.SetAvailability(false);

            // Assert
            staff.IsAvailable.Should().BeFalse();
        }

        #endregion

        #region Qualifications

        [Fact]
        public void AddQualification_ShouldAddWhenNotExisting()
        {
            // Arrange
            var staff = new StaffMember(
                new SMId("ST-006"),
                new SMName("Ana"),
                new Email("ana@porto.com"),
                new PhoneNumber("911222333"),
                new SMOperationalWindow("Mon 08:00-12:00")
            );

            var qId = Guid.NewGuid();

            // Act
            staff.AddQualification(qId);

            // Assert
            staff.QualificationIds.Should().Contain(qId);
        }

        [Fact]
        public void AddQualification_ShouldNotAddDuplicates()
        {
            // Arrange
            var qId = Guid.NewGuid();
            var staff = new StaffMember(
                new SMId("ST-007"),
                new SMName("Rui"),
                new Email("rui@porto.com"),
                new PhoneNumber("933444555"),
                new SMOperationalWindow("Tue-Thu 10:00-16:00"),
                new[] { qId }
            );

            // Act
            staff.AddQualification(qId);

            // Assert
            staff.QualificationIds.Count.Should().Be(1);
        }

        [Fact]
        public void RemoveQualification_ShouldRemoveExisting()
        {
            // Arrange
            var qId = Guid.NewGuid();
            var staff = new StaffMember(
                new SMId("ST-008"),
                new SMName("Clara"),
                new Email("clara@porto.com"),
                new PhoneNumber("955888999"),
                new SMOperationalWindow("Fri 08:00-20:00"),
                new[] { qId }
            );

            // Act
            staff.RemoveQualification(qId);

            // Assert
            staff.QualificationIds.Should().NotContain(qId);
        }

        [Fact]
        public void RemoveQualification_ShouldDoNothingIfNotExisting()
        {
            // Arrange
            var staff = new StaffMember(
                new SMId("ST-009"),
                new SMName("Tiago"),
                new Email("tiago@porto.com"),
                new PhoneNumber("900111222"),
                new SMOperationalWindow("24/7")
            );

            var qId = Guid.NewGuid();

            // Act
            staff.RemoveQualification(qId);

            // Assert
            staff.QualificationIds.Should().BeEmpty();
        }

        #endregion
    }
    
}
