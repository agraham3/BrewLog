using BrewLog.Api.Models;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BrewLog.Api.Tests.Services;

public class AnalyticsServiceTests
{
    private readonly Mock<IBrewSessionRepository> _mockBrewSessionRepository;
    private readonly Mock<ICoffeeBeanRepository> _mockCoffeeBeanRepository;
    private readonly Mock<IGrindSettingRepository> _mockGrindSettingRepository;
    private readonly Mock<IBrewingEquipmentRepository> _mockEquipmentRepository;
    private readonly AnalyticsService _service;

    public AnalyticsServiceTests()
    {
        _mockBrewSessionRepository = new Mock<IBrewSessionRepository>();
        _mockCoffeeBeanRepository = new Mock<ICoffeeBeanRepository>();
        _mockGrindSettingRepository = new Mock<IGrindSettingRepository>();
        _mockEquipmentRepository = new Mock<IBrewingEquipmentRepository>();

        _service = new AnalyticsService(
            _mockBrewSessionRepository.Object,
            _mockCoffeeBeanRepository.Object,
            _mockGrindSettingRepository.Object,
            _mockEquipmentRepository.Object);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WithValidData_ReturnsCorrectStatistics()
    {
        // Arrange
        var coffeeBeans = new List<CoffeeBean>
        {
            new() { Id = 1, Name = "Bean 1", Brand = "Brand A" },
            new() { Id = 2, Name = "Bean 2", Brand = "Brand B" }
        };

        var grindSettings = new List<GrindSetting>
        {
            new() { Id = 1, GrindSize = 15, GrinderType = "Burr" },
            new() { Id = 2, GrindSize = 20, GrinderType = "Blade" }
        };

        var equipment = new List<BrewingEquipment>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express", Type = EquipmentType.EspressoMachine },
            new() { Id = 2, Vendor = "Hario", Model = "V60", Type = EquipmentType.PourOverSetup }
        };

        var brewSessions = new List<BrewSession>
        {
            new()
            {
                Id = 1,
                Method = BrewMethod.Espresso,
                Rating = 8,
                IsFavorite = true,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                CoffeeBeanId = 1,
                BrewingEquipmentId = 1,
                CoffeeBean = coffeeBeans[0],
                BrewingEquipment = equipment[0]
            },
            new()
            {
                Id = 2,
                Method = BrewMethod.Espresso,
                Rating = 9,
                IsFavorite = false,
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                CoffeeBeanId = 1,
                BrewingEquipmentId = 1,
                CoffeeBean = coffeeBeans[0],
                BrewingEquipment = equipment[0]
            },
            new()
            {
                Id = 3,
                Method = BrewMethod.PourOver,
                Rating = 7,
                IsFavorite = true,
                CreatedDate = DateTime.UtcNow.AddDays(-3),
                CoffeeBeanId = 2,
                BrewingEquipmentId = 2,
                CoffeeBean = coffeeBeans[1],
                BrewingEquipment = equipment[1]
            }
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);
        _mockCoffeeBeanRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(coffeeBeans);
        _mockGrindSettingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(grindSettings);
        _mockEquipmentRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(equipment);

        // Act
        var result = await _service.GetDashboardStatsAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalBrewSessions.Should().Be(3);
        result.TotalCoffeeBeans.Should().Be(2);
        result.TotalGrindSettings.Should().Be(2);
        result.TotalEquipment.Should().Be(2);
        result.FavoriteBrews.Should().Be(2);
        result.AverageRating.Should().Be(8.0); // (8 + 9 + 7) / 3

        result.BrewMethodStats.Should().HaveCount(2);
        var espressoStats = result.BrewMethodStats.First(bms => bms.Method == BrewMethod.Espresso);
        espressoStats.Count.Should().Be(2);
        espressoStats.AverageRating.Should().Be(8.5); // (8 + 9) / 2
        espressoStats.FavoriteCount.Should().Be(1);

        result.EquipmentStats.Should().HaveCount(2);
        var brevilleStats = result.EquipmentStats.First(es => es.EquipmentName.Contains("Breville"));
        brevilleStats.UsageCount.Should().Be(2);
        brevilleStats.AverageRating.Should().Be(8.5);

        result.RecentBrews.Should().HaveCount(3);
        result.RecentBrews.First().Id.Should().Be(1); // Most recent
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WithNoData_ReturnsEmptyStatistics()
    {
        // Arrange
        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(new List<BrewSession>());
        _mockCoffeeBeanRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CoffeeBean>());
        _mockGrindSettingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<GrindSetting>());
        _mockEquipmentRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BrewingEquipment>());

        // Act
        var result = await _service.GetDashboardStatsAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalBrewSessions.Should().Be(0);
        result.TotalCoffeeBeans.Should().Be(0);
        result.TotalGrindSettings.Should().Be(0);
        result.TotalEquipment.Should().Be(0);
        result.FavoriteBrews.Should().Be(0);
        result.AverageRating.Should().Be(0);
        result.BrewMethodStats.Should().BeEmpty();
        result.EquipmentStats.Should().BeEmpty();
        result.RecentBrews.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCorrelationAnalysisAsync_WithValidData_ReturnsCorrelations()
    {
        // Arrange
        var brewSessions = new List<BrewSession>
        {
            new()
            {
                Id = 1,
                Rating = 8,
                WaterTemperature = 90m,
                BrewTime = TimeSpan.FromSeconds(30),
                GrindSetting = new GrindSetting { GrindSize = 15 }
            },
            new()
            {
                Id = 2,
                Rating = 9,
                WaterTemperature = 92m,
                BrewTime = TimeSpan.FromSeconds(35),
                GrindSetting = new GrindSetting { GrindSize = 15 }
            },
            new()
            {
                Id = 3,
                Rating = 7,
                WaterTemperature = 85m,
                BrewTime = TimeSpan.FromSeconds(25),
                GrindSetting = new GrindSetting { GrindSize = 20 }
            },
            new()
            {
                Id = 4,
                Rating = 6,
                WaterTemperature = 87m,
                BrewTime = TimeSpan.FromSeconds(28),
                GrindSetting = new GrindSetting { GrindSize = 20 }
            }
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);

        // Act
        var result = await _service.GetCorrelationAnalysisAsync();

        // Assert
        result.Should().NotBeNull();
        result.GrindSizeCorrelations.Should().HaveCount(2);

        var grindSize15 = result.GrindSizeCorrelations.First(c => c.GrindSize == 15);
        grindSize15.AverageRating.Should().Be(8.5); // (8 + 9) / 2
        grindSize15.SampleCount.Should().Be(2);

        var grindSize20 = result.GrindSizeCorrelations.First(c => c.GrindSize == 20);
        grindSize20.AverageRating.Should().Be(6.5); // (7 + 6) / 2
        grindSize20.SampleCount.Should().Be(2);

        result.TemperatureCorrelations.Should().NotBeEmpty();
        result.BrewTimeCorrelations.Should().NotBeEmpty();
        result.OverallCorrelationStrength.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetCorrelationAnalysisAsync_WithNoRatedSessions_ReturnsEmptyCorrelations()
    {
        // Arrange
        var brewSessions = new List<BrewSession>
        {
            new() { Id = 1, Rating = null, GrindSetting = new GrindSetting { GrindSize = 15 } },
            new() { Id = 2, Rating = null, GrindSetting = new GrindSetting { GrindSize = 20 } }
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);

        // Act
        var result = await _service.GetCorrelationAnalysisAsync();

        // Assert
        result.Should().NotBeNull();
        result.GrindSizeCorrelations.Should().BeEmpty();
        result.TemperatureCorrelations.Should().BeEmpty();
        result.BrewTimeCorrelations.Should().BeEmpty();
        result.OverallCorrelationStrength.Should().Be(0);
    }

    [Fact]
    public async Task GetRecommendationsAsync_WithValidData_ReturnsRecommendations()
    {
        // Arrange
        var coffeeBeans = new List<CoffeeBean>
        {
            new() { Id = 1, Name = "Bean 1", Brand = "Brand A" },
            new() { Id = 2, Name = "Bean 2", Brand = "Brand B" }
        };

        var equipment = new List<BrewingEquipment>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express" }
        };

        var brewSessions = new List<BrewSession>
        {
            new()
            {
                Id = 1,
                Rating = 9,
                Method = BrewMethod.Espresso,
                IsFavorite = true,
                CoffeeBeanId = 1,
                BrewingEquipmentId = 1,
                CoffeeBean = coffeeBeans[0],
                BrewingEquipment = equipment[0],
                GrindSetting = new GrindSetting { GrindSize = 15 }
            },
            new()
            {
                Id = 2,
                Rating = 8,
                Method = BrewMethod.Espresso,
                IsFavorite = true,
                CoffeeBeanId = 1,
                BrewingEquipmentId = 1,
                CoffeeBean = coffeeBeans[0],
                BrewingEquipment = equipment[0],
                GrindSetting = new GrindSetting { GrindSize = 15 }
            },
            new()
            {
                Id = 3,
                Rating = 7,
                Method = BrewMethod.PourOver,
                IsFavorite = false,
                CoffeeBeanId = 2,
                CoffeeBean = coffeeBeans[1],
                GrindSetting = new GrindSetting { GrindSize = 20 }
            }
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);

        // Act
        var result = await _service.GetRecommendationsAsync();

        // Assert
        result.Should().NotBeEmpty();
        var recommendations = result.ToList();

        // Should have recommendations for best bean, optimal grind size, best equipment, and favorite combo
        recommendations.Should().Contain(r => r.Type == "BestBean");
        recommendations.Should().Contain(r => r.Type == "OptimalGrindSize");
        recommendations.Should().Contain(r => r.Type == "BestEquipment");
        recommendations.Should().Contain(r => r.Type == "FavoriteCombo");

        // All recommendations should have confidence scores
        recommendations.All(r => r.ConfidenceScore > 0).Should().BeTrue();
    }

    [Fact]
    public async Task GetRecommendationsAsync_WithNoRatedSessions_ReturnsEmptyRecommendations()
    {
        // Arrange
        var brewSessions = new List<BrewSession>
        {
            new() { Id = 1, Rating = null, CoffeeBean = new CoffeeBean { Name = "Test" } }
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);

        // Act
        var result = await _service.GetRecommendationsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEquipmentPerformanceAsync_WithValidData_ReturnsPerformanceMetrics()
    {
        // Arrange
        var equipment = new List<BrewingEquipment>
        {
            new() { Id = 1, Vendor = "Breville", Model = "Barista Express", Type = EquipmentType.EspressoMachine },
            new() { Id = 2, Vendor = "Hario", Model = "V60", Type = EquipmentType.PourOverSetup }
        };

        var brewSessions = new List<BrewSession>
        {
            new()
            {
                Id = 1,
                Rating = 9,
                IsFavorite = true,
                BrewingEquipmentId = 1,
                BrewingEquipment = equipment[0]
            },
            new()
            {
                Id = 2,
                Rating = 8,
                IsFavorite = false,
                BrewingEquipmentId = 1,
                BrewingEquipment = equipment[0]
            },
            new()
            {
                Id = 3,
                Rating = 7,
                IsFavorite = true,
                BrewingEquipmentId = 2,
                BrewingEquipment = equipment[1]
            },
            new()
            {
                Id = 4,
                Rating = null, // No rating
                IsFavorite = false,
                BrewingEquipmentId = 2,
                BrewingEquipment = equipment[1]
            }
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);

        // Act
        var result = await _service.GetEquipmentPerformanceAsync();

        // Assert
        result.Should().NotBeNull();
        result.EquipmentPerformance.Should().HaveCount(2);

        var brevillePerformance = result.EquipmentPerformance.First(ep => ep.Vendor == "Breville");
        brevillePerformance.TotalUses.Should().Be(2);
        brevillePerformance.AverageRating.Should().Be(8.5); // (9 + 8) / 2
        brevillePerformance.FavoriteCount.Should().Be(1);
        brevillePerformance.PerformanceScore.Should().BeGreaterThan(0);

        var harioPerformance = result.EquipmentPerformance.First(ep => ep.Vendor == "Hario");
        harioPerformance.TotalUses.Should().Be(2);
        harioPerformance.AverageRating.Should().Be(7); // Only one rated session
        harioPerformance.FavoriteCount.Should().Be(1);

        result.BestPerformingEquipment.Should().NotBeNull();
        result.MostUsedEquipment.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEquipmentPerformanceAsync_WithNoEquipmentSessions_ReturnsEmptyPerformance()
    {
        // Arrange
        var brewSessions = new List<BrewSession>
        {
            new() { Id = 1, Rating = 8, BrewingEquipmentId = null } // No equipment
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);

        // Act
        var result = await _service.GetEquipmentPerformanceAsync();

        // Assert
        result.Should().NotBeNull();
        result.EquipmentPerformance.Should().BeEmpty();
        result.BestPerformingEquipment.Should().BeNull();
        result.MostUsedEquipment.Should().BeNull();
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WithSessionsWithoutRatings_HandlesCorrectly()
    {
        // Arrange
        var brewSessions = new List<BrewSession>
        {
            new()
            {
                Id = 1,
                Method = BrewMethod.Espresso,
                Rating = 8,
                IsFavorite = true,
                CoffeeBean = new CoffeeBean { Name = "Bean 1", Brand = "Brand A" }
            },
            new()
            {
                Id = 2,
                Method = BrewMethod.Espresso,
                Rating = null, // No rating
                IsFavorite = false,
                CoffeeBean = new CoffeeBean { Name = "Bean 2", Brand = "Brand B" }
            }
        };

        _mockBrewSessionRepository.Setup(r => r.GetWithIncludesAsync()).ReturnsAsync(brewSessions);
        _mockCoffeeBeanRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CoffeeBean>());
        _mockGrindSettingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<GrindSetting>());
        _mockEquipmentRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BrewingEquipment>());

        // Act
        var result = await _service.GetDashboardStatsAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalBrewSessions.Should().Be(2);
        result.AverageRating.Should().Be(8.0); // Only rated sessions counted
        result.BrewMethodStats.Should().HaveCount(1);

        var espressoStats = result.BrewMethodStats.First();
        espressoStats.Count.Should().Be(2); // Both sessions counted
        espressoStats.AverageRating.Should().Be(8.0); // Only rated session counted for average
    }
}