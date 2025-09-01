using BrewLog.Api.DTOs;

namespace BrewLog.Api.Services;

public interface IAnalyticsService
{
    /// <summary>
    /// Gets dashboard statistics including brew method summaries and equipment stats
    /// </summary>
    Task<DashboardStatsDto> GetDashboardStatsAsync();

    /// <summary>
    /// Analyzes correlations between grind size, temperature, brew time and ratings
    /// </summary>
    Task<CorrelationAnalysisDto> GetCorrelationAnalysisAsync();

    /// <summary>
    /// Generates personalized recommendations based on historical data
    /// </summary>
    Task<IEnumerable<RecommendationDto>> GetRecommendationsAsync();

    /// <summary>
    /// Analyzes equipment performance metrics
    /// </summary>
    Task<EquipmentPerformanceDto> GetEquipmentPerformanceAsync();
}