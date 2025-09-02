using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using BrewLog.Api.Controllers;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;

namespace BrewLog.Api.Tests.Controllers;

public class AnalyticsControllerTests
{
    private readonly Mock<IAnalyticsService> _mockService;
    private readonly Mock<ILogger<AnalyticsController>> _mockLogger;
    private readonly AnalyticsController _controller;

    public AnalyticsControllerTests()
    {
        _mockService = new Mock<IAnalyticsService>();
        _mockLogger = new Mock<ILogger<AnalyticsController>>();
        _controller = new AnalyticsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetDashboardStats_ReturnsOkResult_WithDashboardStats()
    {
        // Arrange
        var dashboardStats = new DashboardStatsDto
        {
            TotalBrewSessions = 45,
            TotalCoffeeBeans = 8,
            TotalGrindSettings = 12,
            TotalEquipment = 4,
            FavoriteBrews = 15,
            AverageRating = 7.8,
            BrewMethodStats = new List<BrewMethodStatsDto>
            {
                new() { Method = BrewMethod.PourOver, Count = 18, AverageRating = 8.2, FavoriteCount = 8 },
                new() { Method = BrewMethod.Espresso, Count = 15, AverageRating = 7.9, FavoriteCount = 5 }
            },
            EquipmentStats = new List<EquipmentStatsDto>
            {
                new() { EquipmentId = 1, EquipmentName = "Hario V60-02", Type = EquipmentType.PourOverSetup, UsageCount = 18, AverageRating = 8.2, FavoriteCount = 8 }
            },
            RecentBrews = new List<RecentBrewSessionDto>
            {
                new() { Id = 45, Method = BrewMethod.PourOver, CoffeeBeanName = "Ethiopian Yirgacheffe", Rating = 9, IsFavorite = true, CreatedDate = DateTime.UtcNow }
            }
        };

        _mockService.Setup(s => s.GetDashboardStatsAsync())
                   .ReturnsAsync(dashboardStats);

        // Act
        var result = await _controller.GetDashboardStats();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStats = okResult.Value.Should().BeOfType<DashboardStatsDto>().Subject;
        returnedStats.TotalBrewSessions.Should().Be(45);
        returnedStats.TotalCoffeeBeans.Should().Be(8);
        returnedStats.FavoriteBrews.Should().Be(15);
        returnedStats.AverageRating.Should().Be(7.8);
        returnedStats.BrewMethodStats.Should().HaveCount(2);
        returnedStats.EquipmentStats.Should().HaveCount(1);
        returnedStats.RecentBrews.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetDashboardStats_WithEmptyData_ReturnsOkResult_WithEmptyStats()
    {
        // Arrange
        var emptyStats = new DashboardStatsDto
        {
            TotalBrewSessions = 0,
            TotalCoffeeBeans = 0,
            TotalGrindSettings = 0,
            TotalEquipment = 0,
            FavoriteBrews = 0,
            AverageRating = 0,
            BrewMethodStats = new List<BrewMethodStatsDto>(),
            EquipmentStats = new List<EquipmentStatsDto>(),
            RecentBrews = new List<RecentBrewSessionDto>()
        };

        _mockService.Setup(s => s.GetDashboardStatsAsync())
                   .ReturnsAsync(emptyStats);

        // Act
        var result = await _controller.GetDashboardStats();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStats = okResult.Value.Should().BeOfType<DashboardStatsDto>().Subject;
        returnedStats.TotalBrewSessions.Should().Be(0);
        returnedStats.BrewMethodStats.Should().BeEmpty();
        returnedStats.EquipmentStats.Should().BeEmpty();
        returnedStats.RecentBrews.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCorrelationAnalysis_ReturnsOkResult_WithCorrelationData()
    {
        // Arrange
        var correlationAnalysis = new CorrelationAnalysisDto
        {
            GrindSizeCorrelations = new List<GrindSizeRatingCorrelationDto>
            {
                new() { GrindSize = 15, AverageRating = 8.4, SampleCount = 8 },
                new() { GrindSize = 18, AverageRating = 7.8, SampleCount = 5 }
            },
            TemperatureCorrelations = new List<TemperatureRatingCorrelationDto>
            {
                new() { TemperatureRange = 90.0m, AverageRating = 8.1, SampleCount = 12 },
                new() { TemperatureRange = 95.0m, AverageRating = 7.9, SampleCount = 8 }
            },
            BrewTimeCorrelations = new List<BrewTimeRatingCorrelationDto>
            {
                new() { BrewTimeRange = TimeSpan.FromMinutes(4), AverageRating = 8.3, SampleCount = 10 }
            },
            OverallCorrelationStrength = 0.65
        };

        _mockService.Setup(s => s.GetCorrelationAnalysisAsync())
                   .ReturnsAsync(correlationAnalysis);

        // Act
        var result = await _controller.GetCorrelationAnalysis();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAnalysis = okResult.Value.Should().BeOfType<CorrelationAnalysisDto>().Subject;
        returnedAnalysis.GrindSizeCorrelations.Should().HaveCount(2);
        returnedAnalysis.TemperatureCorrelations.Should().HaveCount(2);
        returnedAnalysis.BrewTimeCorrelations.Should().HaveCount(1);
        returnedAnalysis.OverallCorrelationStrength.Should().Be(0.65);
    }

    [Fact]
    public async Task GetCorrelationAnalysis_WithInsufficientData_ReturnsOkResult_WithEmptyCorrelations()
    {
        // Arrange
        var emptyCorrelationAnalysis = new CorrelationAnalysisDto
        {
            GrindSizeCorrelations = new List<GrindSizeRatingCorrelationDto>(),
            TemperatureCorrelations = new List<TemperatureRatingCorrelationDto>(),
            BrewTimeCorrelations = new List<BrewTimeRatingCorrelationDto>(),
            OverallCorrelationStrength = 0
        };

        _mockService.Setup(s => s.GetCorrelationAnalysisAsync())
                   .ReturnsAsync(emptyCorrelationAnalysis);

        // Act
        var result = await _controller.GetCorrelationAnalysis();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAnalysis = okResult.Value.Should().BeOfType<CorrelationAnalysisDto>().Subject;
        returnedAnalysis.GrindSizeCorrelations.Should().BeEmpty();
        returnedAnalysis.TemperatureCorrelations.Should().BeEmpty();
        returnedAnalysis.BrewTimeCorrelations.Should().BeEmpty();
        returnedAnalysis.OverallCorrelationStrength.Should().Be(0);
    }

    [Fact]
    public async Task GetRecommendations_ReturnsOkResult_WithRecommendations()
    {
        // Arrange
        var recommendations = new List<RecommendationDto>
        {
            new()
            {
                Type = "BestBean",
                Title = "Try Your Best Performing Bean",
                Description = "Ethiopian Yirgacheffe has your highest average rating of 8.7",
                ConfidenceScore = 85.2,
                Parameters = new Dictionary<string, object>
                {
                    ["BeanId"] = 1,
                    ["BeanName"] = "Ethiopian Yirgacheffe",
                    ["AverageRating"] = 8.7
                }
            },
            new()
            {
                Type = "OptimalGrindSize",
                Title = "Optimal Grind Size Found",
                Description = "Grind size 15 produces your best results with an average rating of 8.4",
                ConfidenceScore = 78.9,
                Parameters = new Dictionary<string, object>
                {
                    ["GrindSize"] = 15,
                    ["AverageRating"] = 8.4
                }
            }
        };

        _mockService.Setup(s => s.GetRecommendationsAsync())
                   .ReturnsAsync(recommendations);

        // Act
        var result = await _controller.GetRecommendations();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRecommendations = okResult.Value.Should().BeAssignableTo<IEnumerable<RecommendationDto>>().Subject.ToList();
        returnedRecommendations.Should().HaveCount(2);
        returnedRecommendations[0].Type.Should().Be("BestBean");
        returnedRecommendations[0].ConfidenceScore.Should().Be(85.2);
        returnedRecommendations[1].Type.Should().Be("OptimalGrindSize");
        returnedRecommendations[1].ConfidenceScore.Should().Be(78.9);
    }

    [Fact]
    public async Task GetRecommendations_WithInsufficientData_ReturnsOkResult_WithEmptyRecommendations()
    {
        // Arrange
        var emptyRecommendations = new List<RecommendationDto>();

        _mockService.Setup(s => s.GetRecommendationsAsync())
                   .ReturnsAsync(emptyRecommendations);

        // Act
        var result = await _controller.GetRecommendations();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedRecommendations = okResult.Value.Should().BeAssignableTo<IEnumerable<RecommendationDto>>().Subject;
        returnedRecommendations.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEquipmentPerformance_ReturnsOkResult_WithEquipmentPerformance()
    {
        // Arrange
        var equipmentPerformance = new EquipmentPerformanceDto
        {
            EquipmentPerformance = new List<EquipmentPerformanceItemDto>
            {
                new()
                {
                    EquipmentId = 1,
                    Vendor = "Hario",
                    Model = "V60-02",
                    Type = EquipmentType.PourOverSetup,
                    TotalUses = 18,
                    AverageRating = 8.2,
                    FavoriteCount = 8,
                    PerformanceScore = 87.5
                },
                new()
                {
                    EquipmentId = 2,
                    Vendor = "Breville",
                    Model = "Barista Express",
                    Type = EquipmentType.EspressoMachine,
                    TotalUses = 15,
                    AverageRating = 7.9,
                    FavoriteCount = 5,
                    PerformanceScore = 82.3
                }
            },
            BestPerformingEquipment = new()
            {
                EquipmentId = 1,
                Vendor = "Hario",
                Model = "V60-02",
                Type = EquipmentType.PourOverSetup,
                TotalUses = 18,
                AverageRating = 8.2,
                FavoriteCount = 8,
                PerformanceScore = 87.5
            },
            MostUsedEquipment = new()
            {
                EquipmentId = 1,
                Vendor = "Hario",
                Model = "V60-02",
                Type = EquipmentType.PourOverSetup,
                TotalUses = 18,
                AverageRating = 8.2,
                FavoriteCount = 8,
                PerformanceScore = 87.5
            }
        };

        _mockService.Setup(s => s.GetEquipmentPerformanceAsync())
                   .ReturnsAsync(equipmentPerformance);

        // Act
        var result = await _controller.GetEquipmentPerformance();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPerformance = okResult.Value.Should().BeOfType<EquipmentPerformanceDto>().Subject;
        returnedPerformance.EquipmentPerformance.Should().HaveCount(2);
        returnedPerformance.BestPerformingEquipment.Should().NotBeNull();
        returnedPerformance.BestPerformingEquipment!.EquipmentId.Should().Be(1);
        returnedPerformance.BestPerformingEquipment.PerformanceScore.Should().Be(87.5);
        returnedPerformance.MostUsedEquipment.Should().NotBeNull();
        returnedPerformance.MostUsedEquipment!.TotalUses.Should().Be(18);
    }

    [Fact]
    public async Task GetEquipmentPerformance_WithNoEquipment_ReturnsOkResult_WithEmptyPerformance()
    {
        // Arrange
        var emptyPerformance = new EquipmentPerformanceDto
        {
            EquipmentPerformance = new List<EquipmentPerformanceItemDto>(),
            BestPerformingEquipment = null,
            MostUsedEquipment = null
        };

        _mockService.Setup(s => s.GetEquipmentPerformanceAsync())
                   .ReturnsAsync(emptyPerformance);

        // Act
        var result = await _controller.GetEquipmentPerformance();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPerformance = okResult.Value.Should().BeOfType<EquipmentPerformanceDto>().Subject;
        returnedPerformance.EquipmentPerformance.Should().BeEmpty();
        returnedPerformance.BestPerformingEquipment.Should().BeNull();
        returnedPerformance.MostUsedEquipment.Should().BeNull();
    }

    [Fact]
    public async Task GetDashboardStats_CallsServiceOnce()
    {
        // Arrange
        var dashboardStats = new DashboardStatsDto();
        _mockService.Setup(s => s.GetDashboardStatsAsync())
                   .ReturnsAsync(dashboardStats);

        // Act
        await _controller.GetDashboardStats();

        // Assert
        _mockService.Verify(s => s.GetDashboardStatsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCorrelationAnalysis_CallsServiceOnce()
    {
        // Arrange
        var correlationAnalysis = new CorrelationAnalysisDto();
        _mockService.Setup(s => s.GetCorrelationAnalysisAsync())
                   .ReturnsAsync(correlationAnalysis);

        // Act
        await _controller.GetCorrelationAnalysis();

        // Assert
        _mockService.Verify(s => s.GetCorrelationAnalysisAsync(), Times.Once);
    }

    [Fact]
    public async Task GetRecommendations_CallsServiceOnce()
    {
        // Arrange
        var recommendations = new List<RecommendationDto>();
        _mockService.Setup(s => s.GetRecommendationsAsync())
                   .ReturnsAsync(recommendations);

        // Act
        await _controller.GetRecommendations();

        // Assert
        _mockService.Verify(s => s.GetRecommendationsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetEquipmentPerformance_CallsServiceOnce()
    {
        // Arrange
        var equipmentPerformance = new EquipmentPerformanceDto();
        _mockService.Setup(s => s.GetEquipmentPerformanceAsync())
                   .ReturnsAsync(equipmentPerformance);

        // Act
        await _controller.GetEquipmentPerformance();

        // Assert
        _mockService.Verify(s => s.GetEquipmentPerformanceAsync(), Times.Once);
    }
}