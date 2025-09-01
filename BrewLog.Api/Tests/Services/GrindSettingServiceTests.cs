using AutoMapper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services;
using BrewLog.Api.Services.Exceptions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace BrewLog.Api.Tests.Services;

public class GrindSettingServiceTests
{
    private readonly Mock<IGrindSettingRepository> _mockRepository;
    private readonly Mock<IBrewSessionRepository> _mockBrewSessionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateGrindSettingDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateGrindSettingDto>> _mockUpdateValidator;
    private readonly GrindSettingService _service;

    public GrindSettingServiceTests()
    {
        _mockRepository = new Mock<IGrindSettingRepository>();
        _mockBrewSessionRepository = new Mock<IBrewSessionRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateGrindSettingDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateGrindSettingDto>>();

        _service = new GrindSettingService(
            _mockRepository.Object,
            _mockBrewSessionRepository.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedSetting()
    {
        // Arrange
        var createDto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes"
        };
        var setting = new GrindSetting
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes"
        };
        var createdSetting = new GrindSetting
        {
            Id = 1,
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes",
            CreatedDate = DateTime.UtcNow
        };
        var expectedDto = new GrindSettingResponseDto
        {
            Id = 1,
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes"
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockMapper.Setup(m => m.Map<GrindSetting>(createDto)).Returns(setting);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<GrindSetting>())).ReturnsAsync(createdSetting);
        _mockMapper.Setup(m => m.Map<GrindSettingResponseDto>(createdSetting)).Returns(expectedDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<GrindSetting>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidGrindSize_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateGrindSettingDto
        {
            GrindSize = 0, // Invalid: must be between 1-30
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes"
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Grind size must be between 1 and 30.");
    }

    [Fact]
    public async Task CreateAsync_InvalidGrindWeight_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = -5m, // Invalid: must be positive
            GrinderType = "Burr",
            Notes = "Test notes"
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Grind weight must be greater than 0.");
    }

    [Fact]
    public async Task CreateAsync_InvalidGrindTime_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateGrindSettingDto
        {
            GrindSize = 15,
            GrindTime = TimeSpan.Zero, // Invalid: must be positive
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes"
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Grind time must be greater than 0.");
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ReturnsUpdatedSetting()
    {
        // Arrange
        var updateDto = new UpdateGrindSettingDto
        {
            GrindSize = 20,
            GrindTime = TimeSpan.FromSeconds(45),
            GrindWeight = 25.0m,
            GrinderType = "Blade",
            Notes = "Updated notes"
        };
        var existingSetting = new GrindSetting
        {
            Id = 1,
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Old notes",
            CreatedDate = DateTime.UtcNow.AddDays(-1)
        };
        var updatedSetting = new GrindSetting
        {
            Id = 1,
            GrindSize = 20,
            GrindTime = TimeSpan.FromSeconds(45),
            GrindWeight = 25.0m,
            GrinderType = "Blade",
            Notes = "Updated notes",
            CreatedDate = DateTime.UtcNow.AddDays(-1)
        };
        var expectedDto = new GrindSettingResponseDto
        {
            Id = 1,
            GrindSize = 20,
            GrindTime = TimeSpan.FromSeconds(45),
            GrindWeight = 25.0m,
            GrinderType = "Blade",
            Notes = "Updated notes"
        };

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingSetting);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<GrindSetting>())).ReturnsAsync(updatedSetting);
        _mockMapper.Setup(m => m.Map<GrindSettingResponseDto>(updatedSetting)).Returns(expectedDto);

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        existingSetting.GrindSize.Should().Be(20);
        existingSetting.GrindTime.Should().Be(TimeSpan.FromSeconds(45));
        existingSetting.GrindWeight.Should().Be(25.0m);
        existingSetting.GrinderType.Should().Be("Blade");
        existingSetting.Notes.Should().Be("Updated notes");
    }

    [Fact]
    public async Task DeleteAsync_ExistingIdWithReferences_ThrowsReferentialIntegrityException()
    {
        // Arrange
        var existingSetting = new GrindSetting
        {
            Id = 1,
            GrindSize = 15,
            GrindTime = TimeSpan.FromSeconds(30),
            GrindWeight = 20.5m,
            GrinderType = "Burr",
            Notes = "Test notes"
        };
        var brewSessions = new List<BrewSession>
        {
            new() { Id = 1, GrindSettingId = 1 }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingSetting);
        _mockBrewSessionRepository.Setup(r => r.GetByGrindSettingIdAsync(1)).ReturnsAsync(brewSessions);

        // Act & Assert
        await _service.Invoking(s => s.DeleteAsync(1))
            .Should().ThrowAsync<ReferentialIntegrityException>()
            .WithMessage("Cannot delete grind setting because it is referenced by 1 brew session(s). Please delete the associated brew sessions first.");
    }

    [Fact]
    public async Task GetDistinctGrinderTypesAsync_ReturnsDistinctTypes()
    {
        // Arrange
        var grinderTypes = new List<string> { "Burr", "Blade", "Manual" };
        _mockRepository.Setup(r => r.GetDistinctGrinderTypesAsync()).ReturnsAsync(grinderTypes);

        // Act
        var result = await _service.GetDistinctGrinderTypesAsync();

        // Assert
        result.Should().BeEquivalentTo(grinderTypes);
    }
}