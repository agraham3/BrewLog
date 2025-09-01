using FluentValidation.TestHelper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Validators;
using Xunit;

namespace BrewLog.Api.Tests.Validators;

public class BrewingEquipmentValidatorTests
{
    private readonly CreateBrewingEquipmentDtoValidator _createValidator = new();
    private readonly UpdateBrewingEquipmentDtoValidator _updateValidator = new();

    [Fact]
    public void CreateBrewingEquipmentDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "Pressure", "15 bar" },
                { "WaterTank", "2L" }
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithEmptyVendor_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = []
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Vendor)
            .WithErrorMessage("Vendor is required");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithVendorTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = new string('V', 101), // 101 characters
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = []
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Vendor)
            .WithErrorMessage("Vendor cannot exceed 100 characters");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithEmptyModel_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "",
            Type = EquipmentType.EspressoMachine,
            Specifications = []
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model)
            .WithErrorMessage("Model is required");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithModelTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = new string('M', 101), // 101 characters
            Type = EquipmentType.EspressoMachine,
            Specifications = []
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model)
            .WithErrorMessage("Model cannot exceed 100 characters");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithInvalidEquipmentType_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = (EquipmentType)999, // Invalid enum value
            Specifications = []
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Invalid equipment type");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithNullSpecifications_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = null!
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Specifications cannot be null");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithTooManySpecifications_ShouldHaveValidationError()
    {
        // Arrange
        var specifications = new Dictionary<string, string>();
        for (int i = 0; i < 21; i++) // More than 20 specifications
        {
            specifications.Add($"Spec{i}", $"Value{i}");
        }

        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = specifications
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Cannot have more than 20 specifications");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithEmptySpecificationKey_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "", "15 bar" } // Empty key
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Specification keys and values must not be empty and cannot exceed 100 characters each");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithEmptySpecificationValue_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "Pressure", "" } // Empty value
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Specification keys and values must not be empty and cannot exceed 100 characters each");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithSpecificationKeyTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { new string('K', 101), "15 bar" } // Key too long
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Specification keys and values must not be empty and cannot exceed 100 characters each");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_WithSpecificationValueTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "Pressure", new string('V', 101) } // Value too long
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Specification keys and values must not be empty and cannot exceed 100 characters each");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_EspressoMachineWithValidPressure_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "Pressure", "9 bar" }
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Specifications);
    }

    [Fact]
    public void CreateBrewingEquipmentDto_EspressoMachineWithInvalidPressure_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Breville",
            Model = "Barista Express",
            Type = EquipmentType.EspressoMachine,
            Specifications = new Dictionary<string, string>
            {
                { "Pressure", "high pressure" } // Invalid format
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Espresso machines should have a valid 'Pressure' specification (e.g., '9 bar', '15 bar')");
    }

    [Fact]
    public void CreateBrewingEquipmentDto_GrinderWithValidBurrType_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Baratza",
            Model = "Encore",
            Type = EquipmentType.Grinder,
            Specifications = new Dictionary<string, string>
            {
                { "BurrType", "Conical Steel" }
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Specifications);
    }

    [Fact]
    public void CreateBrewingEquipmentDto_GrinderWithInvalidBurrType_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Baratza",
            Model = "Encore",
            Type = EquipmentType.Grinder,
            Specifications = new Dictionary<string, string>
            {
                { "BurrType", "plastic" } // Invalid burr type
            }
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications)
            .WithErrorMessage("Grinders should have a 'BurrType' specification (e.g., 'Ceramic', 'Steel')");
    }

    [Theory]
    [InlineData(EquipmentType.EspressoMachine)]
    [InlineData(EquipmentType.Grinder)]
    [InlineData(EquipmentType.FrenchPress)]
    [InlineData(EquipmentType.PourOverSetup)]
    [InlineData(EquipmentType.DripMachine)]
    [InlineData(EquipmentType.AeroPress)]
    public void CreateBrewingEquipmentDto_WithValidEquipmentTypes_ShouldNotHaveValidationError(EquipmentType type)
    {
        // Arrange
        var dto = new CreateBrewingEquipmentDto
        {
            Vendor = "Test Vendor",
            Model = "Test Model",
            Type = type,
            Specifications = []
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void UpdateBrewingEquipmentDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new UpdateBrewingEquipmentDto
        {
            Vendor = "Updated Vendor",
            Model = "Updated Model",
            Type = EquipmentType.Grinder,
            Specifications = new Dictionary<string, string>
            {
                { "BurrType", "Ceramic" },
                { "Settings", "40" }
            }
        };

        // Act
        var result = _updateValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}