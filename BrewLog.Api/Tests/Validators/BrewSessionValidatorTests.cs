using FluentAssertions;
using FluentValidation.TestHelper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Validators;
using Xunit;

namespace BrewLog.Api.Tests.Validators;

public class BrewSessionValidatorTests
{
    private readonly CreateBrewSessionDtoValidator _createValidator = new();
    private readonly UpdateBrewSessionDtoValidator _updateValidator = new();

    [Fact]
    public void CreateBrewSessionDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Bright and fruity",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateBrewSessionDto_WithInvalidBrewMethod_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = (BrewMethod)999, // Invalid enum value
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Method)
            .WithErrorMessage("Invalid brew method");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(150)]
    public void CreateBrewSessionDto_WithInvalidWaterTemperature_ShouldHaveValidationError(decimal temperature)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = temperature,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WaterTemperature);
    }

    [Fact]
    public void CreateBrewSessionDto_EspressoWithValidTemperature_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m, // Valid for espresso (88-96)
            BrewTime = TimeSpan.FromSeconds(30),
            TastingNotes = "Rich and creamy",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.WaterTemperature);
    }

    [Theory]
    [InlineData(87)]
    [InlineData(97)]
    public void CreateBrewSessionDto_EspressoWithInvalidTemperature_ShouldHaveValidationError(decimal temperature)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = temperature,
            BrewTime = TimeSpan.FromSeconds(30),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WaterTemperature)
            .WithErrorMessage("Espresso water temperature should be between 88°C and 96°C");
    }

    [Fact]
    public void CreateBrewSessionDto_ColdBrewWithValidTemperature_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.ColdBrew,
            WaterTemperature = 20m, // Valid for cold brew (4-25)
            BrewTime = TimeSpan.FromHours(12),
            TastingNotes = "Smooth and sweet",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.WaterTemperature);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(26)]
    public void CreateBrewSessionDto_ColdBrewWithInvalidTemperature_ShouldHaveValidationError(decimal temperature)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.ColdBrew,
            WaterTemperature = temperature,
            BrewTime = TimeSpan.FromHours(12),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WaterTemperature)
            .WithErrorMessage("Cold brew water temperature should be between 4°C and 25°C");
    }

    [Fact]
    public void CreateBrewSessionDto_WithZeroBrewTime_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.Zero,
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrewTime)
            .WithErrorMessage("Brew time must be greater than zero");
    }

    [Fact]
    public void CreateBrewSessionDto_WithExcessiveBrewTime_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromHours(25), // More than 24 hours
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrewTime)
            .WithErrorMessage("Brew time cannot exceed 24 hours");
    }

    [Fact]
    public void CreateBrewSessionDto_EspressoWithValidBrewTime_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(30), // Valid for espresso (20-40 seconds)
            TastingNotes = "Perfect extraction",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BrewTime);
    }

    [Theory]
    [InlineData(19)]
    [InlineData(41)]
    public void CreateBrewSessionDto_EspressoWithInvalidBrewTime_ShouldHaveValidationError(int seconds)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(seconds),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrewTime)
            .WithErrorMessage("Espresso brew time should be between 20 and 40 seconds");
    }

    [Fact]
    public void CreateBrewSessionDto_FrenchPressWithValidBrewTime_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.FrenchPress,
            WaterTemperature = 94m,
            BrewTime = TimeSpan.FromMinutes(4), // Valid for French press (3-5 minutes)
            TastingNotes = "Full body",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BrewTime);
    }

    [Fact]
    public void CreateBrewSessionDto_WithTastingNotesTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = new string('N', 1001), // 1001 characters
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TastingNotes)
            .WithErrorMessage("Tasting notes cannot exceed 1000 characters");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void CreateBrewSessionDto_WithInvalidRating_ShouldHaveValidationError(int rating)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            Rating = rating,
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating)
            .WithErrorMessage("Rating must be between 1 and 10");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void CreateBrewSessionDto_WithValidRating_ShouldNotHaveValidationError(int rating)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            Rating = rating,
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void CreateBrewSessionDto_WithNullRating_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            Rating = null,
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateBrewSessionDto_WithInvalidCoffeeBeanId_ShouldHaveValidationError(int id)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            CoffeeBeanId = id,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CoffeeBeanId)
            .WithErrorMessage("Coffee bean ID must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateBrewSessionDto_WithInvalidGrindSettingId_ShouldHaveValidationError(int id)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = id
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrindSettingId)
            .WithErrorMessage("Grind setting ID must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateBrewSessionDto_WithInvalidBrewingEquipmentId_ShouldHaveValidationError(int id)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = id
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrewingEquipmentId)
            .WithErrorMessage("Brewing equipment ID must be greater than 0");
    }

    [Fact]
    public void CreateBrewSessionDto_WithNullBrewingEquipmentId_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 93m,
            BrewTime = TimeSpan.FromMinutes(4),
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = null
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BrewingEquipmentId);
    }

    [Fact]
    public void UpdateBrewSessionDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new UpdateBrewSessionDto
        {
            Method = BrewMethod.AeroPress,
            WaterTemperature = 85m,
            BrewTime = TimeSpan.FromMinutes(2),
            TastingNotes = "Updated tasting notes",
            Rating = 9,
            IsFavorite = true,
            CoffeeBeanId = 2,
            GrindSettingId = 2,
            BrewingEquipmentId = 2
        };

        // Act
        var result = _updateValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(BrewMethod.Espresso)]
    [InlineData(BrewMethod.FrenchPress)]
    [InlineData(BrewMethod.PourOver)]
    [InlineData(BrewMethod.Drip)]
    [InlineData(BrewMethod.AeroPress)]
    [InlineData(BrewMethod.ColdBrew)]
    public void CreateBrewSessionDto_WithValidBrewMethods_ShouldNotHaveValidationError(BrewMethod method)
    {
        // Arrange
        var dto = new CreateBrewSessionDto
        {
            Method = method,
            WaterTemperature = 85m, // Generic temperature that works for most methods
            BrewTime = TimeSpan.FromMinutes(3), // Generic time that works for most methods
            TastingNotes = "Test",
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Method);
    }
}