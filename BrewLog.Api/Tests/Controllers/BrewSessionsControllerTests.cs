using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using BrewLog.Api.Controllers;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Services.Exceptions;

namespace BrewLog.Api.Tests.Controllers;

public class BrewSessionsControllerTests
{
    private readonly Mock<IBrewSessionService> _mockService;
    private readonly Mock<ILogger<BrewSessionsController>> _mockLogger;
    private readonly BrewSessionsController _controller;

    public BrewSessionsControllerTests()
    {
        _mockService = new Mock<IBrewSessionService>();
        _mockLogger = new Mock<ILogger<BrewSessionsController>>();
        _controller = new BrewSessionsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetBrewSessions_ReturnsOkResult_WithListOfSessions()
    {
        // Arrange
        var sessions = new List<BrewSessionResponseDto>
        {
            new()
            {
                Id = 1,
                Method = BrewMethod.Espresso,
                WaterTemperature = 93.5m,
                BrewTime = TimeSpan.FromSeconds(25),
                Rating = 8,
                IsFavorite = true,
                CoffeeBeanId = 1,
                GrindSettingId = 1,
                CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
                GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
            },
            new()
            {
                Id = 2,
                Method = BrewMethod.PourOver,
                WaterTemperature = 94.0m,
                BrewTime = TimeSpan.FromMinutes(3),
                Rating = 7,
                IsFavorite = false,
                CoffeeBeanId = 2,
                GrindSettingId = 2,
                CoffeeBean = new CoffeeBeanResponseDto { Id = 2, Name = "Test Bean 2", Brand = "Test Brand 2", RoastLevel = RoastLevel.Light, Origin = "Test Origin 2" },
                GrindSetting = new GrindSettingResponseDto { Id = 2, GrindSize = 20, GrindTime = TimeSpan.FromSeconds(12), GrindWeight = 20.0m, GrinderType = "Blade" }
            }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<BrewSessionFilterDto>()))
                   .ReturnsAsync(sessions);

        // Act
        var result = await _controller.GetBrewSessions();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSessions = okResult.Value.Should().BeAssignableTo<IEnumerable<BrewSessionResponseDto>>().Subject;
        returnedSessions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBrewSessions_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var sessions = new List<BrewSessionResponseDto>
        {
            new()
            {
                Id = 1,
                Method = BrewMethod.Espresso,
                WaterTemperature = 93.5m,
                BrewTime = TimeSpan.FromSeconds(25),
                Rating = 8,
                IsFavorite = true,
                CoffeeBeanId = 1,
                GrindSettingId = 1,
                CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
                GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
            }
        };
        _mockService.Setup(s => s.GetAllAsync(It.Is<BrewSessionFilterDto>(f => 
            f.Method == BrewMethod.Espresso && f.IsFavorite == true)))
                   .ReturnsAsync(sessions);

        // Act
        var result = await _controller.GetBrewSessions(
            method: (int)BrewMethod.Espresso,
            isFavorite: true);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSessions = okResult.Value.Should().BeAssignableTo<IEnumerable<BrewSessionResponseDto>>().Subject;
        returnedSessions.Should().HaveCount(1);
        returnedSessions.First().Method.Should().Be(BrewMethod.Espresso);
        returnedSessions.First().IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task GetBrewSession_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var session = new BrewSessionResponseDto
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 93.5m,
            BrewTime = TimeSpan.FromSeconds(25),
            Rating = 8,
            IsFavorite = true,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
            GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
        };
        _mockService.Setup(s => s.GetByIdAsync(1))
                   .ReturnsAsync(session);

        // Act
        var result = await _controller.GetBrewSession(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSession = okResult.Value.Should().BeOfType<BrewSessionResponseDto>().Subject;
        returnedSession.Id.Should().Be(1);
        returnedSession.Method.Should().Be(BrewMethod.Espresso);
    }

    [Fact]
    public async Task GetBrewSession_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999))
                   .ReturnsAsync((BrewSessionResponseDto?)null);

        // Act
        var result = await _controller.GetBrewSession(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateBrewSession_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 93.5m,
            BrewTime = TimeSpan.FromSeconds(25),
            TastingNotes = "Great espresso",
            Rating = 8,
            IsFavorite = false,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };
        var createdSession = new BrewSessionResponseDto
        {
            Id = 1,
            Method = createDto.Method,
            WaterTemperature = createDto.WaterTemperature,
            BrewTime = createDto.BrewTime,
            TastingNotes = createDto.TastingNotes,
            Rating = createDto.Rating,
            IsFavorite = createDto.IsFavorite,
            CoffeeBeanId = createDto.CoffeeBeanId,
            GrindSettingId = createDto.GrindSettingId,
            BrewingEquipmentId = createDto.BrewingEquipmentId,
            CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
            GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
        };
        _mockService.Setup(s => s.CreateAsync(createDto))
                   .ReturnsAsync(createdSession);

        // Act
        var result = await _controller.CreateBrewSession(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedSession = createdResult.Value.Should().BeOfType<BrewSessionResponseDto>().Subject;
        returnedSession.Method.Should().Be(createDto.Method);
        returnedSession.Rating.Should().Be(createDto.Rating);
        createdResult.ActionName.Should().Be(nameof(BrewSessionsController.GetBrewSession));
    }

    [Fact]
    public async Task CreateBrewSession_WithBusinessValidationError_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 50m, // Invalid temperature for espresso
            BrewTime = TimeSpan.FromSeconds(25),
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };
        _mockService.Setup(s => s.CreateAsync(createDto))
                   .ThrowsAsync(new BusinessValidationException("Water temperature for Espresso should be between 88°C and 96°C."));

        // Act
        var result = await _controller.CreateBrewSession(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateBrewSession_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var updateDto = new UpdateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 94.0m,
            BrewTime = TimeSpan.FromMinutes(3),
            TastingNotes = "Updated notes",
            Rating = 9,
            IsFavorite = true,
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            BrewingEquipmentId = 1
        };
        var updatedSession = new BrewSessionResponseDto
        {
            Id = 1,
            Method = updateDto.Method,
            WaterTemperature = updateDto.WaterTemperature,
            BrewTime = updateDto.BrewTime,
            TastingNotes = updateDto.TastingNotes,
            Rating = updateDto.Rating,
            IsFavorite = updateDto.IsFavorite,
            CoffeeBeanId = updateDto.CoffeeBeanId,
            GrindSettingId = updateDto.GrindSettingId,
            BrewingEquipmentId = updateDto.BrewingEquipmentId,
            CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
            GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
        };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto))
                   .ReturnsAsync(updatedSession);

        // Act
        var result = await _controller.UpdateBrewSession(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSession = okResult.Value.Should().BeOfType<BrewSessionResponseDto>().Subject;
        returnedSession.Method.Should().Be(updateDto.Method);
        returnedSession.Rating.Should().Be(updateDto.Rating);
    }

    [Fact]
    public async Task UpdateBrewSession_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateBrewSessionDto
        {
            Method = BrewMethod.PourOver,
            WaterTemperature = 94.0m,
            BrewTime = TimeSpan.FromMinutes(3),
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };
        _mockService.Setup(s => s.UpdateAsync(999, updateDto))
                   .ThrowsAsync(new NotFoundException("BrewSession", 999));

        // Act
        var result = await _controller.UpdateBrewSession(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateBrewSession_WithBusinessValidationError_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateBrewSessionDto
        {
            Method = BrewMethod.Espresso,
            WaterTemperature = 50m, // Invalid temperature
            BrewTime = TimeSpan.FromSeconds(25),
            CoffeeBeanId = 1,
            GrindSettingId = 1
        };
        _mockService.Setup(s => s.UpdateAsync(1, updateDto))
                   .ThrowsAsync(new BusinessValidationException("Water temperature for Espresso should be between 88°C and 96°C."));

        // Act
        var result = await _controller.UpdateBrewSession(1, updateDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteBrewSession_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(1))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteBrewSession(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteBrewSession_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(999))
                   .ThrowsAsync(new NotFoundException("BrewSession", 999));

        // Act
        var result = await _controller.DeleteBrewSession(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ToggleFavorite_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var updatedSession = new BrewSessionResponseDto
        {
            Id = 1,
            Method = BrewMethod.Espresso,
            WaterTemperature = 93.5m,
            BrewTime = TimeSpan.FromSeconds(25),
            Rating = 8,
            IsFavorite = true, // Toggled to true
            CoffeeBeanId = 1,
            GrindSettingId = 1,
            CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
            GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
        };
        _mockService.Setup(s => s.ToggleFavoriteAsync(1))
                   .ReturnsAsync(updatedSession);

        // Act
        var result = await _controller.ToggleFavorite(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSession = okResult.Value.Should().BeOfType<BrewSessionResponseDto>().Subject;
        returnedSession.Id.Should().Be(1);
        returnedSession.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleFavorite_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.ToggleFavoriteAsync(999))
                   .ThrowsAsync(new NotFoundException("BrewSession", 999));

        // Act
        var result = await _controller.ToggleFavorite(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetFavorites_ReturnsOkResult_WithFavoriteSessions()
    {
        // Arrange
        var favoriteSessions = new List<BrewSessionResponseDto>
        {
            new()
            {
                Id = 1,
                Method = BrewMethod.Espresso,
                WaterTemperature = 93.5m,
                BrewTime = TimeSpan.FromSeconds(25),
                Rating = 8,
                IsFavorite = true,
                CoffeeBeanId = 1,
                GrindSettingId = 1,
                CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
                GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
            }
        };
        _mockService.Setup(s => s.GetFavoritesAsync())
                   .ReturnsAsync(favoriteSessions);

        // Act
        var result = await _controller.GetFavorites();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSessions = okResult.Value.Should().BeAssignableTo<IEnumerable<BrewSessionResponseDto>>().Subject;
        returnedSessions.Should().HaveCount(1);
        returnedSessions.First().IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task GetRecent_WithValidCount_ReturnsOkResult()
    {
        // Arrange
        var recentSessions = new List<BrewSessionResponseDto>
        {
            new()
            {
                Id = 1,
                Method = BrewMethod.Espresso,
                WaterTemperature = 93.5m,
                BrewTime = TimeSpan.FromSeconds(25),
                Rating = 8,
                IsFavorite = false,
                CoffeeBeanId = 1,
                GrindSettingId = 1,
                CreatedDate = DateTime.UtcNow.AddHours(-1),
                CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
                GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
            }
        };
        _mockService.Setup(s => s.GetRecentAsync(5))
                   .ReturnsAsync(recentSessions);

        // Act
        var result = await _controller.GetRecent(5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSessions = okResult.Value.Should().BeAssignableTo<IEnumerable<BrewSessionResponseDto>>().Subject;
        returnedSessions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetRecent_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetRecent(0);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetTopRated_WithValidCount_ReturnsOkResult()
    {
        // Arrange
        var topRatedSessions = new List<BrewSessionResponseDto>
        {
            new()
            {
                Id = 1,
                Method = BrewMethod.Espresso,
                WaterTemperature = 93.5m,
                BrewTime = TimeSpan.FromSeconds(25),
                Rating = 10,
                IsFavorite = true,
                CoffeeBeanId = 1,
                GrindSettingId = 1,
                CoffeeBean = new CoffeeBeanResponseDto { Id = 1, Name = "Test Bean", Brand = "Test Brand", RoastLevel = RoastLevel.Medium, Origin = "Test Origin" },
                GrindSetting = new GrindSettingResponseDto { Id = 1, GrindSize = 15, GrindTime = TimeSpan.FromSeconds(10), GrindWeight = 18.5m, GrinderType = "Burr" }
            }
        };
        _mockService.Setup(s => s.GetTopRatedAsync(5))
                   .ReturnsAsync(topRatedSessions);

        // Act
        var result = await _controller.GetTopRated(5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSessions = okResult.Value.Should().BeAssignableTo<IEnumerable<BrewSessionResponseDto>>().Subject;
        returnedSessions.Should().HaveCount(1);
        returnedSessions.First().Rating.Should().Be(10);
    }

    [Fact]
    public async Task GetTopRated_WithInvalidCount_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetTopRated(101);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}