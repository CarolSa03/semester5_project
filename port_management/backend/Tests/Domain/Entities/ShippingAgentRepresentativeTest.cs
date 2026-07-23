using System;
using PortManagement.Domain.ShippingAgent.Entities;
using Xunit;

namespace PortManagement.Tests.Domain.Entities
{
    public class ShippingAgentRepresentativeTests
    {
        [Fact]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            // Act
            var rep = new ShippingAgentRepresentative();

            // Assert
            Assert.Equal(0, rep.Id);
            Assert.Equal(string.Empty, rep.Name);
            Assert.Equal(string.Empty, rep.Email);
            Assert.Equal(string.Empty, rep.Phone);
            Assert.Equal(string.Empty, rep.ShippingAgentOrganizationId);
            Assert.Null(rep.ShippingAgentOrganization); // null! = referencia não inicializada
        }

        [Fact]
        public void ShouldSetBasicPropertiesCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;

            var rep = new ShippingAgentRepresentative
            {
                Id = 5,
                Name = "Maria Silva",
                Email = "maria@agent.com",
                Phone = "912345678",
                CreatedAt = now,
                ShippingAgentOrganizationId = "ORG100"
            };

            // Assert
            Assert.Equal(5, rep.Id);
            Assert.Equal("Maria Silva", rep.Name);
            Assert.Equal("maria@agent.com", rep.Email);
            Assert.Equal("912345678", rep.Phone);
            Assert.Equal(now, rep.CreatedAt);
            Assert.Equal("ORG100", rep.ShippingAgentOrganizationId);
        }

        [Fact]
        public void ShouldLinkToOrganization()
        {
            // Arrange
            var org = new ShippingAgentOrganization
            {
                Id = "ORG1",
                LegalName = "ShippingCorp"
            };

            var rep = new ShippingAgentRepresentative
            {
                Id = 1,
                Name = "John Doe",
                ShippingAgentOrganization = org,
                ShippingAgentOrganizationId = "ORG1"
            };

            // Assert
            Assert.NotNull(rep.ShippingAgentOrganization);
            Assert.Equal("ORG1", rep.ShippingAgentOrganization.Id);
            Assert.Equal("ShippingCorp", rep.ShippingAgentOrganization.LegalName);
        }

        [Fact]
        public void ShouldAllowUpdatingValues()
        {
            // Arrange
            var rep = new ShippingAgentRepresentative
            {
                Id = 1,
                Name = "Old Name",
                Email = "old@mail.com",
                Phone = "000000000"
            };

            // Act
            rep.Name = "New Name";
            rep.Email = "new@mail.com";
            rep.Phone = "999999999";

            // Assert
            Assert.Equal("New Name", rep.Name);
            Assert.Equal("new@mail.com", rep.Email);
            Assert.Equal("999999999", rep.Phone);
        }
    }
}
