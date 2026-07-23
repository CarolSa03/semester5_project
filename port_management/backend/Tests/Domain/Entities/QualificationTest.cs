using Xunit;
using FluentAssertions;
using PortManagement.Domain.Qualification.Entities;
using PortManagement.Domain.Qualification.ValueObjects;

public class QualificationTests
{
    [Fact]
    public void Can_Set_Code_And_Description()
    {
        var qualification = new Qualification("QUA-001", "STS Crane Operator");

        Assert.Equal("QUA-001", qualification.Code.Value);
        Assert.Equal("STS Crane Operator", qualification.Description.Value);
    }
}
