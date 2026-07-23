using System;
using System.Linq;
using PortManagement.Domain.ShippingAgent.Entities;
using Xunit;

using System;
using System.Linq;
using PortManagement.Domain.ShippingAgent.Entities;
using Xunit;

namespace PortManagement.Tests.Domain.Entities
{
    public class ShippingAgentOrganizationTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepresentativesList()
        {
            // Act
            var org = new ShippingAgentOrganization();

            // Assert
            Assert.NotNull(org.Representatives);
            Assert.Empty(org.Representatives);
        }

        [Fact]
        public void ShouldSetBasicPropertiesCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var org = new ShippingAgentOrganization
            {
                Id = "ORG1",
                LegalName = "Legal Corp",
                AlternativeName = "Alt Corp",
                TaxNumber = "123456789",
                CreatedAt = now
            };

            // Assert
            Assert.Equal("ORG1", org.Id);
            Assert.Equal("Legal Corp", org.LegalName);
            Assert.Equal("Alt Corp", org.AlternativeName);
            Assert.Equal("123456789", org.TaxNumber);
            Assert.Equal(now, org.CreatedAt);
        }

        [Fact]
        public void ShouldAddRepresentativeCorrectly()
        {
            // Arrange
            var org = new ShippingAgentOrganization();
            var rep = new ShippingAgentRepresentative
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "912345678",
                CreatedAt = DateTime.UtcNow,
                ShippingAgentOrganizationId = "ORG1"
            };

            // Act
            org.Representatives.Add(rep);

            // Assert
            Assert.Single(org.Representatives);
            Assert.Equal(1, org.Representatives.First().Id);
            Assert.Equal("John Doe", org.Representatives.First().Name);
        }

        [Fact]
        public void ShouldSupportMultipleRepresentatives()
        {
            // Arrange
            var org = new ShippingAgentOrganization();

            var rep1 = new ShippingAgentRepresentative { Id = 10, Name = "Rep A" };
            var rep2 = new ShippingAgentRepresentative { Id = 20, Name = "Rep B" };

            // Act
            org.Representatives.Add(rep1);
            org.Representatives.Add(rep2);

            // Assert
            Assert.Equal(2, org.Representatives.Count);

            Assert.Contains(org.Representatives, r => r.Id == 10);
            Assert.Contains(org.Representatives, r => r.Id == 20);
        }
    }
}
