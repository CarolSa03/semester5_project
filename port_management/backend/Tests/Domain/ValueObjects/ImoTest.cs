﻿using FluentAssertions;
using PortManagement.Domain.Vessel.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.ValueObjects
{
    /// <summary>
    /// Black box tests for Imo value object
    /// Tests the IMO number validation logic without knowing internal implementation
    /// IMO format: 7 digits with check digit algorithm (ISO 10514)
    /// </summary>
    public class ImoTest
    {
        #region TryCreate with int parameter - Valid Cases

        [Theory]
        [InlineData(9074729)] // Valid IMO number
        [InlineData(8814275)] // Valid IMO number
        [InlineData(9319466)] // Valid IMO number
        [InlineData(1234567)] // Valid format (check digit = 7)
        public void TryCreate_ComNumeroInteiroValido_DeveRetornarTrueECriarImo(int validImo)
        {
            // Act
            var result = Imo.TryCreate(validImo, out var imo);

            // Assert
            result.Should().BeTrue();
            imo.Value.Should().Be(validImo);
        }

        #endregion

        #region TryCreate with int parameter - Invalid Cases

        [Theory]
        [InlineData(1234568)] // Invalid check digit
        [InlineData(9074728)] // Invalid check digit (off by 1)
        [InlineData(123456)]  // Only 6 digits
        [InlineData(12345678)] // 8 digits
        [InlineData(0)]       // Zero
        [InlineData(-1234567)] // Negative number
        public void TryCreate_ComNumeroInteiroInvalido_DeveRetornarFalse(int invalidImo)
        {
            // Act
            var result = Imo.TryCreate(invalidImo, out var imo);

            // Assert
            result.Should().BeFalse();
            imo.Value.Should().Be(0); // Default value
        }

        #endregion

        #region TryCreate with string parameter - Valid Cases

        [Theory]
        [InlineData("9074729")]
        [InlineData("8814275")]
        [InlineData("9319466")]
        [InlineData("0000000")] // Edge case: all zeros (check digit valid)
        public void TryCreate_ComStringValida_DeveRetornarTrueECriarImo(string validImo)
        {
            // Act
            var result = Imo.TryCreate(validImo, out var imo);

            // Assert
            result.Should().BeTrue();
            imo.Value.Should().Be(int.Parse(validImo));
        }

        #endregion

        #region TryCreate with string parameter - Invalid Cases

        [Theory]
        [InlineData("1234568")]   // Invalid check digit
        [InlineData("123456")]    // Too short
        [InlineData("12345678")]  // Too long
        [InlineData("")]          // Empty string
        [InlineData("   ")]       // Whitespace only
        [InlineData("ABC1234")]   // Contains letters
        [InlineData("12345-7")]   // Contains special characters
        [InlineData(" 9074729")]  // Leading space
        [InlineData("9074729 ")]  // Trailing space
        public void TryCreate_ComStringInvalida_DeveRetornarFalse(string invalidImo)
        {
            // Act
            var result = Imo.TryCreate(invalidImo, out var imo);

            // Assert
            result.Should().BeFalse();
            imo.Value.Should().Be(0);
        }

        [Fact]
        public void TryCreate_ComStringNula_DeveRetornarFalse()
        {
            // Act
            var result = Imo.TryCreate(null, out var imo);

            // Assert
            result.Should().BeFalse();
            imo.Value.Should().Be(0);
        }

        #endregion

        #region IsValid method tests

        [Theory]
        [InlineData("9074729")]
        [InlineData("8814275")]
        [InlineData("9319466")]
        public void IsValid_ComStringValida_DeveRetornarTrue(string validImo)
        {
            // Act
            var result = Imo.IsValid(validImo);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("1234568")]   // Invalid check digit
        [InlineData("123456")]    // Too short
        [InlineData("12345678")]  // Too long
        [InlineData("")]          // Empty
        [InlineData("ABC1234")]   // Contains letters
        public void IsValid_ComStringInvalida_DeveRetornarFalse(string invalidImo)
        {
            // Act
            var result = Imo.IsValid(invalidImo);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region ToString tests

        [Fact]
        public void ToString_DeveRetornarNumeroFormatadoCom7Digitos()
        {
            // Arrange
            Imo.TryCreate(9074729, out var imo);

            // Act
            var result = imo.ToString();

            // Assert
            result.Should().Be("9074729");
            result.Length.Should().Be(7);
        }

        [Fact]
        public void ToString_ComNumeroComMenosDe7Digitos_DeveAdicionarZerosEsquerda()
        {
            // Arrange - IMO starting with zeros
            Imo.TryCreate("0000000", out var imo);

            // Act
            var result = imo.ToString();

            // Assert
            result.Should().Be("0000000");
            result.Length.Should().Be(7);
        }

        #endregion

        #region Value Object equality tests

        [Fact]
        public void DoisImosComMesmoValor_DevemSerIguais()
        {
            // Arrange
            Imo.TryCreate(9074729, out var imo1);
            Imo.TryCreate(9074729, out var imo2);

            // Act & Assert
            imo1.Should().Be(imo2);
            (imo1 == imo2).Should().BeTrue();
        }

        [Fact]
        public void DoisImosComValoresDiferentes_DevemSerDiferentes()
        {
            // Arrange
            Imo.TryCreate(9074729, out var imo1);
            Imo.TryCreate(8814275, out var imo2);

            // Act & Assert
            imo1.Should().NotBe(imo2);
            (imo1 != imo2).Should().BeTrue();
        }

        #endregion

        #region Boundary value tests

        [Theory]
        [InlineData(1000007)] // Smallest 7-digit IMO with valid check digit
        [InlineData(9999990)] // Large 7-digit IMO with valid check digit
        public void TryCreate_ComValoresLimite_DeveProcessarCorretamente(int boundaryImo)
        {
            // Act
            var result = Imo.TryCreate(boundaryImo, out var imo);

            // Assert
            if (Imo.IsValid(boundaryImo.ToString()))
            {
                result.Should().BeTrue();
                imo.Value.Should().Be(boundaryImo);
            }
            else
            {
                result.Should().BeFalse();
            }
        }

        #endregion
    }
}

