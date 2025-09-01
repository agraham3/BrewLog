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

public class BrewSessionServiceTests
{
    private readonly Mock<IBrewSessionRepository> _mockRepository;
    private readonly Mock<ICoffeeBeanRepository> _mockCoffeeBeanRepository;
    private readonly Mock<IGrindSettingRepository> _mockGrindSettingRepository;
    private readonly Mock<IBrewingEquipmentRepository> _mockEquipmentRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateBrewSessionDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateBrewSessionDto>> _mockUpdateValidator;
    private readonly BrewSessionService _service;

    public BrewSessionServiceTests()
    {
        _mockRepository = new Mock<IBrewSessionRepository>();
        _mockCoffeeBeanRepository = new Mock<ICoffeeBeanRepository>();
        _mockGrindSettingRepository = new Mock<IGrindSettingRepository>();
        _mockEquipmentRepository = new Mock<IBrewingEquipmentRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateBrewSessionDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateBrewSessionDto>>();

        _service = new BrewSessionService(
            _mockRepository.Object,
            _mockCoffeeBeanRepository.Object,
            _mockGrindSettingRepository.Object,
            _mockEquipmentRepository.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidEspressoSession_ReturnsCreatedSession()
    {
        // Arrange
        var createDto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };
        var session = new BrewSession
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };
        var createdSession = new BrewSession
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1,
            CreatedDate = DateTime.UtcNow
        };
        var sessionWithIncludes = new BrewSession
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1,
            CreatedDate = DateTime.UtcNow,
            CoffeeBean = new CoffeeBean { Id = 1, Name = "Test Bean" },
            GrindSetting = new GrindSetting { Id = 1, GrindSize = 15 },
            BrewingEquipment = new BrewingEquipment { Id = 1, Vendor = "Breville" }
        };
        var expectedDto = new BrewSessionResponseDto
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockCoffeeBeanRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockGrindSettingRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockEquipmentRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map<BrewSession>(createDto)).Returns(session);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<BrewSession>())).ReturnsAsync(createdSession);
        _mockRepository.Setup(r => r.GetByIdWithIncludesAsync(1)).ReturnsAsync(sessionWithIncludes);
        _mockMapper.Setup(m => m.Map<BrewSessionResponseDto>(sessionWithIncludes)).Returns(expectedDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<BrewSession>()), Times.Once);
    }

    [Theory]
    [InlineData(BrewMethod.Espresso, 85, "Water temperature for Espresso should be between 88°C and 96°C.")]
    [InlineData(BrewMethod.Espresso, 100, "Water temperature for Espresso should be between 88°C and 96°C.")]
    [InlineData(BrewMethod.FrenchPress, 85, "Water temperature for FrenchPress should be between 92°C and 96°C.")]
    [InlineData(BrewMethod.ColdBrew, 30, "Water temperature for ColdBrew should be between 4°C and 25°C.")]
    public async Task CreateAsync_InvalidWaterTemperature_ThrowsBusinessValidationException(
        BrewMethod method, decimal temperature, string expectedMessage)
    {
        // Arrange
        var createDto = new CreateBrewSessionDto
        {
            Method = method,
            WaterTemperature = temperature,
            BrewTime = TimeSpan.FromMinutes(2),
            TastingNotes = "Test",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockCoffeeBeanRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockGrindSettingRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockEquipmentRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [InlineData(BrewMethod.Espresso, 10, "Brew time for Espresso should be between 0.3 and 0.7 minutes.")]
    [InlineData(BrewMethod.Espresso, 60, "Brew time for Espresso should be between 0.3 and 0.7 minutes.")]
    [InlineData(BrewMethod.FrenchPress, 120, "Brew time for FrenchPress should be between 3.0 and 5.0 minutes.")]
    [InlineData(BrewMethod.FrenchPress, 400, "Brew time for FrenchPress should be between 3.0 and 5.0 minutes.")]
    public async Task CreateAsync_InvalidBrewTime_ThrowsBusinessValidationException(
        BrewMethod method, int timeInSeconds, string expectedMessage)
    {
        // Arrange
        var createDto = new CreateBrewSessionDto
        {
            Method = method,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(timeInSeconds),
            TastingNotes = "Test",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockCoffeeBeanRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockGrindSettingRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockEquipmentRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [InlineData(0, "Rating must be between 1 and 10.")]
    [InlineData(11, "Rating must be between 1 and 10.")]
    public async Task CreateAsync_InvalidRating_ThrowsBusinessValidationException(int rating, string expectedMessage)
    {
        // Arrange
        var createDto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Test",
            Rating = rating,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockCoffeeBeanRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockGrindSettingRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockEquipmentRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public async Task CreateAsync_NonExistentCoffeeBean_ThrowsBusinessValidationException()
    {
        // Arrange
        var createDto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Test",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 999, // Non-existent
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default)).ReturnsAsync(new ValidationResult());
        _mockCoffeeBeanRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<BusinessValidationException>()
            .WithMessage("Coffee bean with ID 999 does not exist.");
    }

    [Fact]
    public async Task ToggleFavoriteAsync_ExistingSession_TogglesAndReturnsUpdatedSession()
    {
        // Arrange
        var existingSession = new BrewSession
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };
        var updatedSession = new BrewSession
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = true, // Toggled
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };
        var sessionWithIncludes = new BrewSession
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = true,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1,
            CoffeeBean = new CoffeeBean { Id = 1, Name = "Test Bean" },
            GrindSetting = new GrindSetting { Id = 1, GrindSize = 15 },
            BrewingEquipment = new BrewingEquipment { Id = 1, Vendor = "Breville" }
        };
        var expectedDto = new BrewSessionResponseDto
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 92m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Rich and bold",
            Rating = 8,
            IsFavorite = true,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingSession);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<BrewSession>())).ReturnsAsync(updatedSession);
        _mockRepository.Setup(r => r.GetByIdWithIncludesAsync(1)).ReturnsAsync(sessionWithIncludes);
        _mockMapper.Setup(m => m.Map<BrewSessionResponseDto>(sessionWithIncludes)).Returns(expectedDto);

        // Act
        var result = await _service.ToggleFavoriteAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        existingSession.IsFavorite.Should().BeTrue();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<BrewSession>()), Times.Once);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_NonExistentSession_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((BrewSession?)null);

        // Act & Assert
        await _service.Invoking(s => s.ToggleFavoriteAsync(999))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("BrewSession with ID 999 was not found.");
    }

    [Fact]
    public async Task GetFavoritesAsync_ReturnsFavoriteSessionsOnly()
    {
        // Arrange
        var favoriteSessions = new List<BrewSession>
        {
            new() { Id = 1, IsFavorite = true, Method = BrewMethod.Espresso },
            new() { Id = 3, IsFavorite = true, Method = BrewMethod.PourOver }
        };
        var expectedDtos = new List<BrewSessionResponseDto>
        {
            new() { Id = 1, IsFavorite = true, Method = BrewMethod.Espresso },
            new() { Id = 3, IsFavorite = true, Method = BrewMethod.PourOver }
        };

        _mockRepository.Setup(r => r.GetFavoritesAsync()).ReturnsAsync(favoriteSessions);
        _mockMapper.Setup(m => m.Map<IEnumerable<BrewSessionResponseDto>>(favoriteSessions)).Returns(expectedDtos);

        // Act
        var result = await _service.GetFavoritesAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
        result.All(s => s.IsFavorite).Should().BeTrue();
    }
}