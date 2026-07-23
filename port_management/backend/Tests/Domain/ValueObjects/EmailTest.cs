using FluentAssertions;
using PortManagement.Domain.Staff.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.ValueObjects
{
    /// <summary>
    /// Black box tests for Email value object
    /// Tests the email validation logic without knowing internal implementation
    /// Email format: local-part@domain with basic RFC-compliant validation
    /// </summary>
    public class EmailTest
    {
        #region Create - Valid Cases

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test.email@domain.com")]
        [InlineData("user+tag@example.co.uk")]
        [InlineData("firstname.lastname@company.org")]
        [InlineData("user_name@example-domain.com")]
        [InlineData("123@example.com")]
        [InlineData("a@b.c")]
        [InlineData("test@subdomain.example.com")]
        public void Create_ComEmailValido_DeveCriarEmail(string validEmail)
        {
            // Act
            var email = new Email(validEmail);

            // Assert
            email.Should().NotBeNull();
            email.Value.Should().Be(validEmail.Trim().ToLowerInvariant());
        }

        [Theory]
        [InlineData("  user@example.com  ", "user@example.com")]
        [InlineData("USER@EXAMPLE.COM", "user@example.com")]
        [InlineData("  ADMIN@Domain.COM  ", "admin@domain.com")]
        public void Create_ComEmailComEspacosOuMaiusculas_DeveNormalizarEmail(
            string inputEmail,
            string expectedEmail)
        {
            // Act
            var email = new Email(inputEmail);

            // Assert
            email.Value.Should().Be(expectedEmail);
        }

        #endregion

        #region Create - Invalid Cases

        [Theory]
        [InlineData("")] // Empty string
        [InlineData("   ")] // Whitespace only
        [InlineData(null)] // Null
        public void Create_ComEmailVazioOuNulo_DeveLancarArgumentException(string? invalidEmail)
        {
            // Act
            var act = () => new Email(invalidEmail!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Email address cannot be empty.*");
        }

        [Theory]
        [InlineData("invalid")] // Missing @ and domain
        [InlineData("@example.com")] // Missing local part
        [InlineData("user@")] // Missing domain
        [InlineData("user@domain")] // Missing TLD
        [InlineData("user @example.com")] // Space in local part
        [InlineData("user@exam ple.com")] // Space in domain
        [InlineData("user@@example.com")] // Double @
        [InlineData("user@example@com")] // Multiple @
        [InlineData("user.example.com")] // Missing @
        [InlineData("user@.com")] // Domain starts with dot
        [InlineData("user@example.")] // Domain ends with dot
        public void Create_ComFormatoInvalido_DeveLancarArgumentException(string invalidEmail)
        {
            // Act
            var act = () => new Email(invalidEmail);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid email format*");
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_ComMesmoEndereco_DeveSerIguais()
        {
            // Arrange
            var email1 = new Email("user@example.com");
            var email2 = new Email("user@example.com");

            // Assert
            email1.Should().Be(email2);
            (email1 == email2).Should().BeTrue();
            email1.GetHashCode().Should().Be(email2.GetHashCode());
        }

        [Fact]
        public void Equals_ComEnderecosDiferentes_NaoDeveSerIguais()
        {
            // Arrange
            var email1 = new Email("user@example.com");
            var email2 = new Email("other@example.com");

            // Assert
            email1.Should().NotBe(email2);
            (email1 != email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_ComNormalizacao_DeveSerIguais()
        {
            // Arrange
            var email1 = new Email("USER@EXAMPLE.COM");
            var email2 = new Email("user@example.com");
            var email3 = new Email("  user@example.com  ");

            // Assert
            email1.Should().Be(email2);
            email2.Should().Be(email3);
            email1.Should().Be(email3);
        }

        #endregion

        #region ToString Tests

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test@domain.org")]
        public void ToString_DeveRetornarEndereco(string emailAddress)
        {
            // Arrange
            var email = new Email(emailAddress);

            // Act
            var result = email.ToString();

            // Assert
            result.Should().Be(emailAddress.ToLowerInvariant());
        }

        #endregion

        #region Immutability Tests

        [Fact]
        public void Email_DeveSerImutavel()
        {
            // Arrange
            var originalAddress = "user@example.com";
            var email = new Email(originalAddress);

            // Assert
            email.Value.Should().Be(originalAddress);
        }

        #endregion

        #region Edge Cases

        [Theory]
        [InlineData("a@b.c")] // Minimum valid length
        [InlineData("verylongemailaddress@verylongdomainname.com")] // Long but valid
        public void Create_ComCasosLimite_DeveFuncionarCorretamente(string edgeCaseEmail)
        {
            // Act
            var email = new Email(edgeCaseEmail);

            // Assert
            email.Should().NotBeNull();
            email.Value.Should().Be(edgeCaseEmail.ToLowerInvariant());
        }

        #endregion
    }
}