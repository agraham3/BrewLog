using FluentAssertions;
using FluentValidation.TestHelper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Validators;
using Xunit;

namespace BrewLog.Api.Tests.Validators;

public class CoffeeBeanValidatorTests
{
    private readonly CreateCoffeeBeanDtoValidator _createValidator = new();
    private readonly UpdateCoffeeBeanDtoValidator _updateValidator = new();

    [Fact]
    public void CreateCoffeeBeanDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "Ethiopian Yirgacheffe",
            Brand = "Blue Bottle",
            RoastLevel = RoastLevel.Light,
            Origin = "Ethiopia"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateCoffeeBeanDto_WithEmptyName_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "",
            Brand = "Blue Bottle",
            RoastLevel = RoastLevel.Light,
            Origin = "Ethiopia"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Coffee bean name is required");
    }

    [Fact]
    public void CreateCoffeeBeanDto_WithNameTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = new string('A', 101), // 101 characters
            Brand = "Blue Bottle",
            RoastLevel = RoastLevel.Light,
            Origin = "Ethiopia"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Coffee bean name cannot exceed 100 characters");
    }

    [Fact]
    public void CreateCoffeeBeanDto_WithEmptyBrand_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "Ethiopian Yirgacheffe",
            Brand = "",
            RoastLevel = RoastLevel.Light,
            Origin = "Ethiopia"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Brand)
            .WithErrorMessage("Brand is required");
    }

    [Fact]
    public void CreateCoffeeBeanDto_WithBrandTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "Ethiopian Yirgacheffe",
            Brand = new string('B', 101), // 101 characters
            RoastLevel = RoastLevel.Light,
            Origin = "Ethiopia"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Brand)
            .WithErrorMessage("Brand cannot exceed 100 characters");
    }

    [Fact]
    public void CreateCoffeeBeanDto_WithInvalidRoastLevel_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "Ethiopian Yirgacheffe",
            Brand = "Blue Bottle",
            RoastLevel = (RoastLevel)999, // Invalid enum value
            Origin = "Ethiopia"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoastLevel)
            .WithErrorMessage("Invalid RoastLevel. Accepted values: Light, MediumLight, Medium, MediumDark, Dark");
    }

    [Fact]
    public void CreateCoffeeBeanDto_WithEmptyOrigin_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "Ethiopian Yirgacheffe",
            Brand = "Blue Bottle",
            RoastLevel = RoastLevel.Light,
            Origin = ""
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Origin)
            .WithErrorMessage("Origin is required");
    }

    [Fact]
    public void CreateCoffeeBeanDto_WithOriginTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "Ethiopian Yirgacheffe",
            Brand = "Blue Bottle",
            RoastLevel = RoastLevel.Light,
            Origin = new string('O', 201) // 201 characters
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Origin)
            .WithErrorMessage("Origin cannot exceed 200 characters");
    }

    [Fact]
    public void UpdateCoffeeBeanDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new UpdateCoffeeBeanDto
        {
            Name = "Ethiopian Yirgacheffe",
            Brand = "Blue Bottle",
            RoastLevel = RoastLevel.Medium,
            Origin = "Ethiopia, Yirgacheffe Region"
        };

        // Act
        var result = _updateValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(RoastLevel.Light)]
    [InlineData(RoastLevel.MediumLight)]
    [InlineData(RoastLevel.Medium)]
    [InlineData(RoastLevel.MediumDark)]
    [InlineData(RoastLevel.Dark)]
    public void CreateCoffeeBeanDto_WithValidRoastLevels_ShouldNotHaveValidationErrors(RoastLevel roastLevel)
    {
        // Arrange
        var dto = new CreateCoffeeBeanDto
        {
            Name = "Test Bean",
            Brand = "Test Brand",
            RoastLevel = roastLevel,
            Origin = "Test Origin"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RoastLevel);
    }
}