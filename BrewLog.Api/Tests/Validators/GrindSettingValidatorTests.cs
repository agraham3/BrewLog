using FluentAssertions;
using FluentValidation.TestHelper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Validators;
using Xunit;

namespace BrewLog.Api.Tests.Validators;

public class GrindSettingValidatorTests
{
    private readonly CreateGrindSettingDtoValidator _createValidator = new();
    private readonly UpdateGrindSettingDtoValidator _updateValidator = new();

    [Fact]
    public void CreateGrindSettingDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = "Perfect for pour over"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(31)]
    [InlineData(100)]
    public void CreateGrindSettingDto_WithInvalidGrindSize_ShouldHaveValidationError(int grindSize)
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = grindSize,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrindSize)
            .WithErrorMessage("Grind size must be between 1 and 30");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(30)]
    public void CreateGrindSettingDto_WithValidGrindSize_ShouldNotHaveValidationError(int grindSize)
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = grindSize,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.GrindSize);
    }

    [Fact]
    public void CreateGrindSettingDto_WithZeroGrindTime_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.Zero,
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrindTime)
            .WithErrorMessage("Grind time must be greater than zero");
    }

    [Fact]
    public void CreateGrindSettingDto_WithExcessiveGrindTime_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromMinutes(11), // More than 10 minutes
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrindTime)
            .WithErrorMessage("Grind time cannot exceed 10 minutes");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void CreateGrindSettingDto_WithInvalidGrindWeight_ShouldHaveValidationError(decimal weight)
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = weight,
            GrinderType = "Burr Grinder",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrindWeight)
            .WithErrorMessage("Grind weight must be greater than zero");
    }

    [Fact]
    public void CreateGrindSettingDto_WithExcessiveGrindWeight_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 1001m, // More than 1000 grams
            GrinderType = "Burr Grinder",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrindWeight)
            .WithErrorMessage("Grind weight cannot exceed 1000 grams");
    }

    [Fact]
    public void CreateGrindSettingDto_WithEmptyGrinderType_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrinderType)
            .WithErrorMessage("Grinder type is required");
    }

    [Fact]
    public void CreateGrindSettingDto_WithGrinderTypeTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = new string('G', 101), // 101 characters
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GrinderType)
            .WithErrorMessage("Grinder type cannot exceed 100 characters");
    }

    [Fact]
    public void CreateGrindSettingDto_WithNotesTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = new string('N', 501) // 501 characters
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Notes)
            .WithErrorMessage("Notes cannot exceed 500 characters");
    }

    [Fact]
    public void CreateGrindSettingDto_WithEmptyNotes_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = ""
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void UpdateGrindSettingDto_WithValidData_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var dto = new UpdateGrindSettingDto
        {
            GrindSize = 20,
            GrindTime = TimeSpan.FromSeconds(45),
            GrindWeight = 25.0m,
            GrinderType = "Conical Burr Grinder",
            Notes = "Updated settings for espresso"
        };

        // Act
        var result = _updateValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1, 30)]
    [InlineData(15, 45)]
    [InlineData(30, 60)]
    public void CreateGrindSettingDto_WithValidTimeRanges_ShouldNotHaveValidationError(int grindSize, int seconds)
    {
        // Arrange
        var dto = new CreateGrindSettingDto
        {
            GrindSize = grindSize,
            GrindTime = TimeSpan.FromSeconds(seconds),
            GrindWeight = 20.5m,
            GrinderType = "Burr Grinder",
            Notes = "Test"
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.GrindTime);
    }
}