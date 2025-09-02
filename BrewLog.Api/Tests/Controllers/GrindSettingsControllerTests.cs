using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using BrewLog.Api.Controllers;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Services.Exceptions;

namespace BrewLog.Api.Tests.Controllers;

public class GrindSettingsControllerTests
{
    private readonly Mock<IGrindSettingService> _mockService;
    private readonly Mock<ILogger<GrindSettingsController>> _mockLogger;
    private readonly GrindSettingsController _controller;

    public GrindSettingsControllerTests()
    {
        _mockService = new Mock<IGrindSettingService>();
        _mockLogger = new Mock<ILogger<GrindSettingsController>>();
        _controller = new GrindSettingsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetGrindSettings_ReturnsOkResult_WithListOfSettings()
    {
        // Arrange
        var settings = new List<GrindSettingResponseDto>
        {
            new() { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(30), GrindWeight = 20.5m, GrinderType = "Burr", Notes = "Medium grind" },
            new() { Id = 2, GrindSize = 10, GrindTime = TimeSpan.FromSeconds(25), GrindWeight = 18.0m, GrinderType = "Blade", Notes = "Fine grind" }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GrindSettingFilterDto>()))
                   .ReturnsAsync(settings);

        // Act
        var result = await _controller.GetGrindSettings();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSettings = okResult.Value.Should().BeAssignableTo<IEnumerable<GrindSettingResponseDto>>().Subject;
        returnedSettings.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGrindSettings_WithFilters_PassesFiltersToService()
    {
        // Arrange
        var settings = new List<GrindSettingResponseDto>();
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GrindSettingFilterDto>()))
                   .ReturnsAsync(settings);

        // Act
        await _controller.GetGrindSettings(minGrindSize: 10, maxGrindSize: 20, grinderType: "Burr");

        // Assert
        _mockService.Verify(s => s.GetAllAsync(It.Is<GrindSettingFilterDto>(f => 
            f.MinGrindSize == 10 && 
            f.MaxGrindSize == 20 && 
            f.GrinderType == "Burr")), Times.Once);
    }

    [Fact]
    public async Task GetGrindSetting_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var setting = new GrindSettingResponseDto
        {
            Id = 1,
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Medium grind"
        };
        _mockService.Setup(s => s.GetByIdAsync(1))
                   .ReturnsAsync(setting);

        // Act
        var result = await _controller.GetGrindSetting(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSetting = okResult.Value.Should().BeOfType<GrindSettingResponseDto>().Subject;
        returnedSetting.Id.Should().Be(1);
        returnedSetting.GrindSize.Should().Be(15);
        returnedSetting.GrinderType.Should().Be("Burr");
    }

    [Fact]
    public async Task GetGrindSetting_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999))
                   .ReturnsAsync((GrindSettingResponseDto?)null);

        // Act
        var result = await _controller.GetGrindSetting(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateGrindSetting_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Medium grind for pour over"
        };
        var createdSetting = new GrindSettingResponseDto
        {
            Id = 1,
            GrindSize = createDto.GrindSize,
            GrindTime = createDto.GrindTime,
            GrindWeight = createDto.GrindWeight,
            GrinderType = createDto.GrinderType,
            Notes = createDto.Notes
        };
        _mockService.Setup(s => s.CreateAsync(createDto))
                   .ReturnsAsync(createdSetting);

        // Act
        var result = await _controller.CreateGrindSetting(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedSetting = createdResult.Value.Should().BeOfType<GrindSettingResponseDto>().Subject;
        returnedSetting.GrindSize.Should().Be(createDto.GrindSize);
        returnedSetting.GrinderType.Should().Be(createDto.GrinderType);
        createdResult.ActionName.Should().Be(nameof(GrindSettingsController.GetGrindSetting));
    }

    [Fact]
    public async Task UpdateGrindSetting_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var updateDto = new UpdateGrindSettingDto
        {
            GrindSize = 18,
            GrindTime = TimeSpan.FromSeconds(35),
            GrindWeight = 22.0m,
            GrinderType = "Burr",
            Notes = "Updated medium-coarse grind"
        };
        var updatedSetting = new GrindSettingResponseDto
        {
            Id = 1,
            GrindSize = updateDto.GrindSize,
            GrindTime = updateDto.GrindTime,
            GrindWeight = updateDto.GrindWeight,
            GrinderType = updateDto.GrinderType,
            Notes = updateDto.Notes
        };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto))
                   .ReturnsAsync(updatedSetting);

        // Act
        var result = await _controller.UpdateGrindSetting(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSetting = okResult.Value.Should().BeOfType<GrindSettingResponseDto>().Subject;
        returnedSetting.GrindSize.Should().Be(updateDto.GrindSize);
        returnedSetting.Notes.Should().Be(updateDto.Notes);
    }

    [Fact]
    public async Task UpdateGrindSetting_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateGrindSettingDto
        {
            GrindSize = 18,
            GrindTime = TimeSpan.FromSeconds(35),
            GrindWeight = 22.0m,
            GrinderType = "Burr",
            Notes = "Updated grind"
        };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto))
                   .ThrowsAsync(new NotFoundException("GrindSetting", 999));

        // Act
        var result = await _controller.UpdateGrindSetting(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteGrindSetting_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGrindSetting(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteGrindSetting_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(999))
                   .ThrowsAsync(new NotFoundException("GrindSetting", 999));

        // Act
        var result = await _controller.DeleteGrindSetting(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteGrindSetting_WithActiveBrewSessions_ReturnsConflict()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
                   .ThrowsAsync(new InvalidOperationException("Cannot delete grind setting with active brew sessions"));

        // Act
        var result = await _controller.DeleteGrindSetting(1);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task GetRecentGrindSettings_WithValidCount_ReturnsOkResult()
    {
        // Arrange
        var settings = new List<GrindSettingResponseDto>
        {
            new() { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(30), GrindWeight = 20.5m, GrinderType = "Burr", Notes = "Recent 1" },
            new() { Id = 2, GrindSize = 12, GrindTime = TimeSpan.FromSeconds(28), GrindWeight = 19.0m, GrinderType = "Burr", Notes = "Recent 2" }
        };
        _mockService.Setup(s => s.GetRecentlyUsedAsync(5))
                   .ReturnsAsync(settings);

        // Act
        var result = await _controller.GetRecentGrindSettings(5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSettings = okResult.Value.Should().BeAssignableTo<IEnumerable<GrindSettingResponseDto>>().Subject;
        returnedSettings.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentGrindSettings_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetRecentGrindSettings(0);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetMostUsedGrindSettings_WithValidCount_ReturnsOkResult()
    {
        // Arrange
        var settings = new List<GrindSettingResponseDto>
        {
            new() { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(30), GrindWeight = 20.5m, GrinderType = "Burr", Notes = "Popular 1" },
            new() { Id = 2, GrindSize = 12, GrindTime = TimeSpan.FromSeconds(28), GrindWeight = 19.0m, GrinderType = "Burr", Notes = "Popular 2" }
        };
        _mockService.Setup(s => s.GetMostUsedAsync(5))
                   .ReturnsAsync(settings);

        // Act
        var result = await _controller.GetMostUsedGrindSettings(5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSettings = okResult.Value.Should().BeAssignableTo<IEnumerable<GrindSettingResponseDto>>().Subject;
        returnedSettings.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMostUsedGrindSettings_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetMostUsedGrindSettings(101);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetGrinderTypes_ReturnsOkResult_WithListOfTypes()
    {
        // Arrange
        var grinderTypes = new List<string> { "Burr", "Blade", "Manual" };
        _mockService.Setup(s => s.GetDistinctGrinderTypesAsync())
                   .ReturnsAsync(grinderTypes);

        // Act
        var result = await _controller.GetGrinderTypes();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTypes = okResult.Value.Should().BeAssignableTo<IEnumerable<string>>().Subject;
        returnedTypes.Should().HaveCount(3);
        returnedTypes.Should().Contain("Burr");
        returnedTypes.Should().Contain("Blade");
        returnedTypes.Should().Contain("Manual");
    }

    [Theory]
    [InlineData(10, 20, "Burr")]
    [InlineData(5, 15, "Blade")]
    [InlineData(null, null, null)]
    public async Task GetGrindSettings_WithDifferentFilters_CallsServiceWithCorrectParameters(
        int? minGrindSize, int? maxGrindSize, string? grinderType)
    {
        // Arrange
        var settings = new List<GrindSettingResponseDto>();
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GrindSettingFilterDto>()))
                   .ReturnsAsync(settings);

        // Act
        await _controller.GetGrindSettings(minGrindSize: minGrindSize, maxGrindSize: maxGrindSize, grinderType: grinderType);

        // Assert
        _mockService.Verify(s => s.GetAllAsync(It.Is<GrindSettingFilterDto>(f => 
            f.MinGrindSize == minGrindSize && 
            f.MaxGrindSize == maxGrindSize && 
            f.GrinderType == grinderType)), Times.Once);
    }

    [Fact]
    public async Task GetGrindSettings_WithWeightFilters_PassesWeightFiltersToService()
    {
        // Arrange
        var settings = new List<GrindSettingResponseDto>();
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GrindSettingFilterDto>()))
                   .ReturnsAsync(settings);

        // Act
        await _controller.GetGrindSettings(minGrindWeight: 15.0m, maxGrindWeight: 25.0m);

        // Assert
        _mockService.Verify(s => s.GetAllAsync(It.Is<GrindSettingFilterDto>(f => 
            f.MinGrindWeight == 15.0m && 
            f.MaxGrindWeight == 25.0m)), Times.Once);
    }

    [Fact]
    public async Task GetGrindSettings_WithDateFilters_PassesDateFiltersToService()
    {
        // Arrange
        var settings = new List<GrindSettingResponseDto>();
        var createdAfter = DateTime.Now.AddDays(-7);
        var createdBefore = DateTime.Now;
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GrindSettingFilterDto>()))
                   .ReturnsAsync(settings);

        // Act
        await _controller.GetGrindSettings(createdAfter: createdAfter, createdBefore: createdBefore);

        // Assert
        _mockService.Verify(s => s.GetAllAsync(It.Is<GrindSettingFilterDto>(f => 
            f.CreatedAfter == createdAfter && 
            f.CreatedBefore == createdBefore)), Times.Once);
    }
}