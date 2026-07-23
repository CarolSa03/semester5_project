using FluentAssertions;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;

namespace Tests.Domain.Entities;

public class CrewInfoTest
{
    [Fact]
    public void DeveCriarCrewInfoComDadosValidos()
    {
        // Arrange & Act
        var crewInfo = new CrewInfo
        {
            CaptainName = new CICaptainName("Captain John Smith"),
            CrewCount = new CICrewCount(25),
            SafetyOfficers = new List<CrewMember>
            {
                new CrewMember
                {
                    Name = new CICrewMemberName("Jane Doe"),
                    CitizenId = new CICitizenId("123456789"),
                    Nationality = new CINationality("USA")
                },
                new CrewMember
                {
                    Name = new CICrewMemberName("Dr. Smith"),
                    CitizenId = new CICitizenId("987654321"),
                    Nationality = new CINationality("UK")
                }
            }
        };

        // Assert
        crewInfo.CaptainName.Value.Should().Be("Captain John Smith");
        crewInfo.CrewCount.Value.Should().Be(25);
        crewInfo.SafetyOfficers.Should().HaveCount(2);
    }

    [Fact]
    public void CrewMember_DeveTerPropriedadesCorretas()
    {
        // Arrange & Act
        var crewMember = new CrewMember
        {
            Name = new CICrewMemberName("John Doe"),
            CitizenId = new CICitizenId("123456789"),
            Nationality = new CINationality("Portugal")
        };

        // Assert
        crewMember.Name.Value.Should().Be("John Doe");
        crewMember.CitizenId.Value.Should().Be("123456789");
        crewMember.Nationality.Value.Should().Be("Portugal");
    }

    [Fact]
    public void CrewInfo_DevePermitirListasVazias()
    {
        // Arrange & Act
        var crewInfo = new CrewInfo
        {
            CaptainName = new CICaptainName("Captain"),
            CrewCount = new CICrewCount(1),
            SafetyOfficers = new List<CrewMember>()
        };

        // Assert
        crewInfo.SafetyOfficers.Should().BeEmpty();
    }

    [Fact]
    public void CrewInfo_DevePermitirVariosOfficers()
    {
        // Arrange & Act
        var crewInfo = new CrewInfo
        {
            CaptainName = new CICaptainName("Captain Smith"),
            CrewCount = new CICrewCount(50),
            SafetyOfficers = new List<CrewMember>
            {
                new CrewMember { Name = new CICrewMemberName("Officer 1"), CitizenId = new CICitizenId("1111"), Nationality = new CINationality("USA") },
                new CrewMember { Name = new CICrewMemberName("Officer 2"), CitizenId = new CICitizenId("2222"), Nationality = new CINationality("UK") },
                new CrewMember { Name = new CICrewMemberName("Officer 3"), CitizenId = new CICitizenId("3333"), Nationality = new CINationality("Canada") }
            }
        };

        // Assert
        crewInfo.SafetyOfficers.Should().HaveCount(3);
    }

    [Fact]
    public void IsCompliant_DeveRetornarTrueComDadosValidos()
    {
        // Arrange
        var crewInfo = new CrewInfo
        {
            CaptainName = new CICaptainName("Captain"),
            CrewCount = new CICrewCount(10),
            SafetyOfficers = new List<CrewMember>
            {
                new CrewMember
                {
                    Name = new CICrewMemberName("Officer"),
                    CitizenId = new CICitizenId("1234"),
                    Nationality = new CINationality("USA")
                }
            }
        };

        // Act
        var result = crewInfo.IsCompliant();

        // Assert
        result.Should().BeTrue();
    }
}

