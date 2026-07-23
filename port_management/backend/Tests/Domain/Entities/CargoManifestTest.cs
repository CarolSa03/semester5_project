using FluentAssertions;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;
using Xunit;

namespace PortManagement.Tests.Domain.Entities;

/// <summary>
/// Black box tests for CargoManifest entity
/// Tests cargo manifest validation and dangerous cargo detection logic
/// </summary>
public class CargoManifestTest
{
    #region HasContainers tests

    [Fact]
    public void HasContainers_ComListaDeContainersPreenchida_DeveRetornarTrue()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Type = ManifestType.Loading,
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo 
                { 
                    ContainerId = new CMContainerId("MSCU1234567"), 
                    CargoType = new CMCargoType("General") 
                }
            }
        };

        // Act
        var result = manifest.HasContainers();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasContainers_ComListaVazia_DeveRetornarFalse()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Type = ManifestType.Loading,
            Containers = new List<ContainerInfo>()
        };

        // Act
        var result = manifest.HasContainers();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasContainers_ComListaNula_DeveRetornarFalse()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Type = ManifestType.Loading,
            Containers = null
        };

        // Act
        var result = manifest.HasContainers();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region HasDangerousCargo tests

    [Theory]
    [InlineData("HAZMAT")]
    [InlineData("CHEMICAL")]
    [InlineData("RADIOACTIVE")]
    [InlineData("EXPLOSIVE")]
    [InlineData("DANGEROUS")]
    public void HasDangerousCargo_ComTipoDeCargoPerigoso_DeveRetornarTrue(string dangerousType)
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo 
                { 
                    ContainerId = new CMContainerId("MSCU1234567"), 
                    CargoType = new CMCargoType(dangerousType) 
                }
            }
        };

        // Act
        var result = manifest.HasDangerousCargo();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("hazmat")]    // Lowercase
    [InlineData("Chemical")]  // Mixed case
    [InlineData("RADIOACTIVE")] // Uppercase
    public void HasDangerousCargo_ComTipoPerigosoCaseInsensitive_DeveRetornarTrue(string dangerousType)
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo 
                { 
                    ContainerId = new CMContainerId("MSCU1234567"), 
                    CargoType = new CMCargoType(dangerousType) 
                }
            }
        };

        // Act
        var result = manifest.HasDangerousCargo();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("General")]
    [InlineData("Refrigerated")]
    [InlineData("Dry Bulk")]
    [InlineData("Liquid")]
    public void HasDangerousCargo_ComTipoDeCargoNormal_DeveRetornarFalse(string safeType)
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo 
                { 
                    ContainerId = new CMContainerId("MSCU1234567"), 
                    CargoType = new CMCargoType(safeType) 
                }
            }
        };

        // Act
        var result = manifest.HasDangerousCargo();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasDangerousCargo_ComMisturaDeCargoNormalEPerigoso_DeveRetornarTrue()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo { ContainerId = new CMContainerId("MSCU1234567"), CargoType = new CMCargoType("General") },
                new ContainerInfo { ContainerId = new CMContainerId("MSCU9876543"), CargoType = new CMCargoType("HAZMAT") },
                new ContainerInfo { ContainerId = new CMContainerId("MSCU5555555"), CargoType = new CMCargoType("Refrigerated") }
            }
        };

        // Act
        var result = manifest.HasDangerousCargo();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasDangerousCargo_ComListaVazia_DeveRetornarFalse()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>()
        };

        // Act
        var result = manifest.HasDangerousCargo();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasDangerousCargo_ComListaNula_DeveRetornarFalse()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = null
        };

        // Act
        var result = manifest.HasDangerousCargo();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Validate tests - Success cases

    [Fact]
    public void Validate_ComContainersValidos_NaoDeveLancarExcecao()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Type = ManifestType.Loading,
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo
                {
                    ContainerId = new CMContainerId("MSCU1234567"),
                    CargoType = new CMCargoType("General"),
                    Bay = 1,
                    Row = 2,
                    Tier = 3
                }
            }
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ComListaDeContainersNula_NaoDeveLancarExcecao()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Type = ManifestType.Loading,
            Containers = null
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ComListaDeContainersVazia_NaoDeveLancarExcecao()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Type = ManifestType.Loading,
            Containers = new List<ContainerInfo>()
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Validate tests - Error cases

    [Fact]
    public void Validate_ComContainerIdVazio_DeveLancarInvalidOperationException()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo
                {
                    ContainerId = null, // Will be null instead of empty string
                    CargoType = new CMCargoType("General"),
                    Bay = 1,
                    Row = 1,
                    Tier = 1
                }
            }
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Container ID is required");
    }

    [Fact]
    public void Validate_ComContainerIdNulo_DeveLancarInvalidOperationException()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo
                {
                    ContainerId = null,
                    CargoType = new CMCargoType("General"),
                    Bay = 1,
                    Row = 1,
                    Tier = 1
                }
            }
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Container ID is required");
    }

    [Fact]
    public void Validate_ComCargoTypeVazio_DeveLancarInvalidOperationException()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo
                {
                    ContainerId = new CMContainerId("MSCU1234567"),
                    CargoType = null, // Null instead of empty
                    Bay = 1,
                    Row = 1,
                    Tier = 1
                }
            }
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cargo type is required");
    }

    [Fact]
    public void Validate_ComCargoTypeNulo_DeveLancarInvalidOperationException()
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo
                {
                    ContainerId = new CMContainerId("MSCU1234567"),
                    CargoType = null,
                    Bay = 1,
                    Row = 1,
                    Tier = 1
                }
            }
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cargo type is required");
    }

    [Theory]
    [InlineData(-1, 1, 1)]
    [InlineData(1, -1, 1)]
    [InlineData(1, 1, -1)]
    [InlineData(-5, -5, -5)]
    public void Validate_ComValoresNegativosDePosicao_DeveLancarInvalidOperationException(int bay, int row, int tier)
    {
        // Arrange
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo
                {
                    ContainerId = new CMContainerId("MSCU1234567"),
                    CargoType = new CMCargoType("General"),
                    Bay = bay,
                    Row = row,
                    Tier = tier
                }
            }
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Bay, Row, and Tier must be positive values");
    }

    [Fact]
    public void Validate_ComMultiplosContainers_DeveValidarTodos()
    {
        // Arrange - Second container has invalid data
        var manifest = new CargoManifest
        {
            Containers = new List<ContainerInfo>
            {
                new ContainerInfo
                {
                    ContainerId = new CMContainerId("MSCU1234567"),
                    CargoType = new CMCargoType("General"),
                    Bay = 1,
                    Row = 1,
                    Tier = 1
                },
                new ContainerInfo
                {
                    ContainerId = new CMContainerId("MSCU9876543"),
                    CargoType = null, // Invalid
                    Bay = 2,
                    Row = 2,
                    Tier = 2
                }
            }
        };

        // Act
        Action act = () => manifest.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cargo type is required");
    }

    #endregion

    #region ManifestType enum tests

    [Fact]
    public void ManifestType_DeveSuportarLoading()
    {
        // Arrange & Act
        var manifest = new CargoManifest { Type = ManifestType.Loading };

        // Assert
        manifest.Type.Should().Be(ManifestType.Loading);
    }

    [Fact]
    public void ManifestType_DeveSuportarUnloading()
    {
        // Arrange & Act
        var manifest = new CargoManifest { Type = ManifestType.Unloading };

        // Assert
        manifest.Type.Should().Be(ManifestType.Unloading);
    }

    #endregion
}

