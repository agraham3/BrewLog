using Microsoft.AspNetCore.Mvc;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Attributes;

namespace BrewLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard statistics including brew method summaries and equipment stats
    /// </summary>
    /// <returns>Dashboard statistics with brew method stats, equipment stats, and recent brews</returns>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "DashboardStatsSuccess", "Successfully retrieved dashboard statistics")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        _logger.LogInformation("Getting dashboard statistics");

        var dashboardStats = await _analyticsService.GetDashboardStatsAsync();
        
        _logger.LogInformation("Successfully retrieved dashboard statistics: {TotalSessions} sessions, {TotalBeans} beans, {FavoriteBrews} favorites",
            dashboardStats.TotalBrewSessions, dashboardStats.TotalCoffeeBeans, dashboardStats.FavoriteBrews);

        return Ok(dashboardStats);
    }

    /// <summary>
    /// Analyze correlations between grind size, temperature, brew time and ratings
    /// </summary>
    /// <returns>Correlation analysis showing relationships between brewing parameters and ratings</returns>
    [HttpGet("correlations")]
    [ProducesResponseType(typeof(CorrelationAnalysisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "CorrelationAnalysisSuccess", "Successfully retrieved correlation analysis")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<CorrelationAnalysisDto>> GetCorrelationAnalysis()
    {
        _logger.LogInformation("Getting correlation analysis");

        var correlationAnalysis = await _analyticsService.GetCorrelationAnalysisAsync();
        
        _logger.LogInformation("Successfully retrieved correlation analysis: {GrindSizeCorrelations} grind size correlations, {TemperatureCorrelations} temperature correlations, {BrewTimeCorrelations} brew time correlations",
            correlationAnalysis.GrindSizeCorrelations.Count, correlationAnalysis.TemperatureCorrelations.Count, correlationAnalysis.BrewTimeCorrelations.Count);

        return Ok(correlationAnalysis);
    }

    /// <summary>
    /// Generate personalized recommendations based on historical data
    /// </summary>
    /// <returns>List of personalized recommendations for optimal brewing based on user's historical data</returns>
    [HttpGet("recommendations")]
    [ProducesResponseType(typeof(IEnumerable<RecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "RecommendationsSuccess", "Successfully retrieved personalized recommendations")]
    [SwaggerResponseExample(200, "RecommendationsEmpty", "Empty recommendations when insufficient data")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<IEnumerable<RecommendationDto>>> GetRecommendations()
    {
        _logger.LogInformation("Getting personalized recommendations");

        var recommendations = await _analyticsService.GetRecommendationsAsync();
        var recommendationsList = recommendations.ToList();
        
        _logger.LogInformation("Successfully retrieved {Count} personalized recommendations", recommendationsList.Count);

        return Ok(recommendationsList);
    }

    /// <summary>
    /// Analyze equipment performance metrics
    /// </summary>
    /// <returns>Equipment performance analysis including usage stats, ratings, and performance scores</returns>
    [HttpGet("equipment-performance")]
    [ProducesResponseType(typeof(EquipmentPerformanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "EquipmentPerformanceSuccess", "Successfully retrieved equipment performance analysis")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<EquipmentPerformanceDto>> GetEquipmentPerformance()
    {
        _logger.LogInformation("Getting equipment performance analysis");

        var equipmentPerformance = await _analyticsService.GetEquipmentPerformanceAsync();
        
        _logger.LogInformation("Successfully retrieved equipment performance analysis: {EquipmentCount} equipment analyzed, best performing: {BestPerforming}, most used: {MostUsed}",
            equipmentPerformance.EquipmentPerformance.Count,
            equipmentPerformance.BestPerformingEquipment?.Model ?? "None",
            equipmentPerformance.MostUsedEquipment?.Model ?? "None");

        return Ok(equipmentPerformance);
    }
}