using FluentAssertions;
using PortManagement.Domain.Staff.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.ValueObjects
{
    /// <summary>
    /// Black box tests for PhoneNumber value object
    /// Tests the phone number validation logic without knowing internal implementation
    /// Phone format: 7-15 characters, can include +, digits, spaces and hyphens
    /// </summary>
    public class PhoneNumberTest
    {
        #region Create - Valid Cases

        [Theory]
        [InlineData("+351912345678")] // Portuguese mobile with country code
        [InlineData("912345678")] // Portuguese mobile without country code
        [InlineData("+1-555-123-4567")] // US format with hyphens (15 chars)
        [InlineData("+44 20 7946 0958")] // UK format with spaces (16 chars total, mas só conta dígitos+símbolo = 14)
        [InlineData("1234567")] // Minimum length (7 digits)
        [InlineData("+12345678901234")] // Maximum length (15 characters)
        [InlineData("123-456-7890")] // Format with hyphens
        [InlineData("123 456 7890")] // Format with spaces
        [InlineData("+351 912345678")] // Portuguese with space
        [InlineData("+1 555-123-4567")] // Mixed spaces and hyphens (15 chars)
        public void Create_ComNumeroValido_DeveCriarPhoneNumber(string validPhone)
        {
            // Act
            var phoneNumber = new PhoneNumber(validPhone);

            // Assert
            phoneNumber.Should().NotBeNull();
            phoneNumber.Value.Should().Be(validPhone.Trim());
        }

        [Theory]
        [InlineData("  +351912345678  ", "+351912345678")]
        [InlineData("  123-456-7890  ", "123-456-7890")]
        [InlineData("   1234567   ", "1234567")]
        public void Create_ComEspacosNoInicio_DeveRemoverEspacos(
            string inputPhone,
            string expectedPhone)
        {
            // Act
            var phoneNumber = new PhoneNumber(inputPhone);

            // Assert
            phoneNumber.Value.Should().Be(expectedPhone);
        }

        #endregion

        #region Create - Invalid Cases

        [Theory]
        [InlineData("")] // Empty string
        [InlineData("   ")] // Whitespace only
        [InlineData(null)] // Null
        public void Create_ComNumeroVazioOuNulo_DeveLancarArgumentException(string? invalidPhone)
        {
            // Act
            var act = () => new PhoneNumber(invalidPhone!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Phone number cannot be empty.*");
        }

        [Theory]
        [InlineData("123456")] // Too short (6 digits)
        [InlineData("+1234567890123456")] // Too long (17 characters)
        [InlineData("abc1234567")] // Contains letters
        [InlineData("123@456-7890")] // Invalid character @
        [InlineData("123.456.7890")] // Contains dots (not allowed)
        [InlineData("(123) 456-7890")] // Contains parentheses (not allowed)
        [InlineData("++351912345678")] // Double plus sign
        [InlineData("351+912345678")] // Plus sign in middle
        [InlineData("12345#67890")] // Contains # symbol
        [InlineData("123*456*7890")] // Contains * symbol
        public void Create_ComFormatoInvalido_DeveLancarArgumentException(string invalidPhone)
        {
            // Act
            var act = () => new PhoneNumber(invalidPhone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid phone number format.*");
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_ComMesmoNumero_DeveSerIguais()
        {
            // Arrange
            var phone1 = new PhoneNumber("+351912345678");
            var phone2 = new PhoneNumber("+351912345678");

            // Assert
            phone1.Should().Be(phone2);
            (phone1 == phone2).Should().BeTrue();
            phone1.GetHashCode().Should().Be(phone2.GetHashCode());
        }

        [Fact]
        public void Equals_ComNumerosDiferentes_NaoDeveSerIguais()
        {
            // Arrange
            var phone1 = new PhoneNumber("+351912345678");
            var phone2 = new PhoneNumber("+351987654321");

            // Assert
            phone1.Should().NotBe(phone2);
            (phone1 != phone2).Should().BeTrue();
        }

        [Fact]
        public void Equals_ComFormatosDiferentes_NaoDeveSerIguais()
        {
            // Arrange
            var phone1 = new PhoneNumber("912345678");
            var phone2 = new PhoneNumber("91 234 5678");
            var phone3 = new PhoneNumber("912-345-678");

            // Assert
            phone1.Should().NotBe(phone2);
            phone1.Should().NotBe(phone3);
            phone2.Should().NotBe(phone3);
        }

        [Fact]
        public void Equals_ComEspacosRemovidos_DeveSerIguais()
        {
            // Arrange
            var phone1 = new PhoneNumber("  +351912345678  ");
            var phone2 = new PhoneNumber("+351912345678");

            // Assert
            phone1.Should().Be(phone2);
        }

        #endregion

        #region ToString Tests

        [Theory]
        [InlineData("+351912345678")]
        [InlineData("123-456-7890")]
        [InlineData("+1 555-123-456")]
        public void ToString_DeveRetornarNumero(string phoneNumber)
        {
            // Arrange
            var phone = new PhoneNumber(phoneNumber);

            // Act
            var result = phone.ToString();

            // Assert
            result.Should().Be(phoneNumber);
        }

        #endregion

        #region Immutability Tests

        [Fact]
        public void PhoneNumber_DeveSerImutavel()
        {
            // Arrange
            var originalNumber = "+351912345678";
            var phoneNumber = new PhoneNumber(originalNumber);

            // Assert
            phoneNumber.Value.Should().Be(originalNumber);
        }

        #endregion

        #region Edge Cases

        [Theory]
        [InlineData("1234567")] // Minimum valid length (7 characters)
        [InlineData("+12345678901234")] // Maximum valid length (15 characters)
        [InlineData("+1-2-3-4-5-6-7")] // Many hyphens (15 chars)
        [InlineData("+1 2 3 4 5 6 7")] // Many spaces (14 chars)
        public void Create_ComCasosLimite_DeveFuncionarCorretamente(string edgeCasePhone)
        {
            // Act
            var phoneNumber = new PhoneNumber(edgeCasePhone);

            // Assert
            phoneNumber.Should().NotBeNull();
            phoneNumber.Value.Should().Be(edgeCasePhone);
        }

        [Theory]
        [InlineData("123456")] // Just below minimum (6 characters)
        [InlineData("+1234567890123456")] // Just above maximum (17 characters)
        public void Create_ComTamanhoForaDoLimite_DeveLancarArgumentException(string invalidPhone)
        {
            // Act
            var act = () => new PhoneNumber(invalidPhone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid phone number format.*");
        }

        #endregion

        #region International Formats

        [Theory]
        [InlineData("+351912345678")] // Portugal
        [InlineData("+1-555-123-456")] // USA (15 chars)
        [InlineData("+44 2079460958")] // UK (14 chars)
        [InlineData("+49 3012345678")] // Germany (14 chars)
        [InlineData("+86 1012345678")] // China (14 chars)
        [InlineData("+91 2212345678")] // India (14 chars)
        public void Create_ComFormatosInternacionais_DeveCriarPhoneNumber(string internationalPhone)
        {
            // Act
            var phoneNumber = new PhoneNumber(internationalPhone);

            // Assert
            phoneNumber.Should().NotBeNull();
            phoneNumber.Value.Should().Be(internationalPhone);
        }

        #endregion

        #region Portuguese Specific Cases

        [Theory]
        [InlineData("912345678")] // Mobile without prefix
        [InlineData("961234567")] // Mobile without prefix (96x)
        [InlineData("213456789")] // Landline Lisbon area
        [InlineData("229876543")] // Landline Porto area
        [InlineData("+351 912345678")] // Mobile with country code and space
        [InlineData("+351-912345678")] // Mobile with country code and hyphen
        public void Create_ComNumerosPortugueses_DeveCriarPhoneNumber(string portuguesePhone)
        {
            // Act
            var phoneNumber = new PhoneNumber(portuguesePhone);

            // Assert
            phoneNumber.Should().NotBeNull();
            phoneNumber.Value.Should().Be(portuguesePhone);
        }

        #endregion
    }
}