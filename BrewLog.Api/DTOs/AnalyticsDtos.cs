using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

/// <summary>
/// Dashboard statistics containing overall metrics and summaries
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Total number of brew sessions recorded
    /// </summary>
    public int TotalBrewSessions { get; set; }
    
    /// <summary>
    /// Total number of coffee beans in the system
    /// </summary>
    public int TotalCoffeeBeans { get; set; }
    
    /// <summary>
    /// Total number of grind settings configured
    /// </summary>
    public int TotalGrindSettings { get; set; }
    
    /// <summary>
    /// Total number of brewing equipment items
    /// </summary>
    public int TotalEquipment { get; set; }
    
    /// <summary>
    /// Number of brew sessions marked as favorites
    /// </summary>
    public int FavoriteBrews { get; set; }
    
    /// <summary>
    /// Average rating across all brew sessions
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Statistics grouped by brew method
    /// </summary>
    public List<BrewMethodStatsDto> BrewMethodStats { get; set; } = [];
    
    /// <summary>
    /// Statistics for brewing equipment usage
    /// </summary>
    public List<EquipmentStatsDto> EquipmentStats { get; set; } = [];
    
    /// <summary>
    /// Most recent brew sessions
    /// </summary>
    public List<RecentBrewSessionDto> RecentBrews { get; set; } = [];
}

/// <summary>
/// Statistics for a specific brew method
/// </summary>
public class BrewMethodStatsDto
{
    /// <summary>
    /// The brew method (Espresso, FrenchPress, PourOver, etc.)
    /// </summary>
    public BrewMethod Method { get; set; }
    
    /// <summary>
    /// Number of sessions using this brew method
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// Average rating for sessions using this brew method
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Number of favorite sessions using this brew method
    /// </summary>
    public int FavoriteCount { get; set; }
}

/// <summary>
/// Usage statistics for brewing equipment
/// </summary>
public class EquipmentStatsDto
{
    /// <summary>
    /// Unique identifier of the equipment
    /// </summary>
    public int EquipmentId { get; set; }
    
    /// <summary>
    /// Name of the equipment
    /// </summary>
    public string EquipmentName { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of equipment (EspressoMachine, Grinder, etc.)
    /// </summary>
    public EquipmentType Type { get; set; }
    
    /// <summary>
    /// Number of times this equipment has been used
    /// </summary>
    public int UsageCount { get; set; }
    
    /// <summary>
    /// Average rating for sessions using this equipment
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Number of favorite sessions using this equipment
    /// </summary>
    public int FavoriteCount { get; set; }
}

/// <summary>
/// Summary information for recent brew sessions
/// </summary>
public class RecentBrewSessionDto
{
    /// <summary>
    /// Unique identifier of the brew session
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Brew method used for this session
    /// </summary>
    public BrewMethod Method { get; set; }
    
    /// <summary>
    /// Name of the coffee bean used
    /// </summary>
    public string CoffeeBeanName { get; set; } = string.Empty;
    
    /// <summary>
    /// Rating given to this brew session (1-10, null if not rated)
    /// </summary>
    public int? Rating { get; set; }
    
    /// <summary>
    /// Whether this session is marked as a favorite
    /// </summary>
    public bool IsFavorite { get; set; }
    
    /// <summary>
    /// Date and time when the session was created
    /// </summary>
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Analysis of correlations between brewing parameters and ratings
/// </summary>
public class CorrelationAnalysisDto
{
    /// <summary>
    /// Correlation data between grind size and ratings
    /// </summary>
    public List<GrindSizeRatingCorrelationDto> GrindSizeCorrelations { get; set; } = [];
    
    /// <summary>
    /// Correlation data between water temperature and ratings
    /// </summary>
    public List<TemperatureRatingCorrelationDto> TemperatureCorrelations { get; set; } = [];
    
    /// <summary>
    /// Correlation data between brew time and ratings
    /// </summary>
    public List<BrewTimeRatingCorrelationDto> BrewTimeCorrelations { get; set; } = [];
    
    /// <summary>
    /// Overall correlation strength across all parameters (0-1, where 1 is perfect correlation)
    /// </summary>
    public double OverallCorrelationStrength { get; set; }
}

/// <summary>
/// Correlation data between grind size and average rating
/// </summary>
public class GrindSizeRatingCorrelationDto
{
    /// <summary>
    /// Grind size on a 1-30 scale (1=finest, 30=coarsest)
    /// </summary>
    public int GrindSize { get; set; }
    
    /// <summary>
    /// Average rating for sessions using this grind size
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Number of sessions contributing to this data point
    /// </summary>
    public int SampleCount { get; set; }
}

/// <summary>
/// Correlation data between water temperature and average rating
/// </summary>
public class TemperatureRatingCorrelationDto
{
    /// <summary>
    /// Water temperature range in Celsius
    /// </summary>
    public decimal TemperatureRange { get; set; }
    
    /// <summary>
    /// Average rating for sessions using this temperature range
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Number of sessions contributing to this data point
    /// </summary>
    public int SampleCount { get; set; }
}

/// <summary>
/// Correlation data between brew time and average rating
/// </summary>
public class BrewTimeRatingCorrelationDto
{
    /// <summary>
    /// Brew time range
    /// </summary>
    public TimeSpan BrewTimeRange { get; set; }
    
    /// <summary>
    /// Average rating for sessions using this brew time range
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Number of sessions contributing to this data point
    /// </summary>
    public int SampleCount { get; set; }
}

/// <summary>
/// Personalized brewing recommendation based on historical data
/// </summary>
public class RecommendationDto
{
    /// <summary>
    /// Type of recommendation (e.g., "grind_size", "temperature", "brew_time")
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Short title describing the recommendation
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the recommendation
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Confidence score for this recommendation (0-1, where 1 is highest confidence)
    /// </summary>
    public double ConfidenceScore { get; set; }
    
    /// <summary>
    /// Additional parameters specific to this recommendation
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = [];
}

/// <summary>
/// Analysis of brewing equipment performance metrics
/// </summary>
public class EquipmentPerformanceDto
{
    /// <summary>
    /// Performance metrics for all equipment
    /// </summary>
    public List<EquipmentPerformanceItemDto> EquipmentPerformance { get; set; } = [];
    
    /// <summary>
    /// Equipment with the highest performance score
    /// </summary>
    public EquipmentPerformanceItemDto? BestPerformingEquipment { get; set; }
    
    /// <summary>
    /// Most frequently used equipment
    /// </summary>
    public EquipmentPerformanceItemDto? MostUsedEquipment { get; set; }
}

/// <summary>
/// Performance metrics for a specific piece of brewing equipment
/// </summary>
public class EquipmentPerformanceItemDto
{
    /// <summary>
    /// Unique identifier of the equipment
    /// </summary>
    public int EquipmentId { get; set; }
    
    /// <summary>
    /// Equipment vendor/manufacturer
    /// </summary>
    public string Vendor { get; set; } = string.Empty;
    
    /// <summary>
    /// Equipment model name
    /// </summary>
    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of equipment (EspressoMachine, Grinder, etc.)
    /// </summary>
    public EquipmentType Type { get; set; }
    
    /// <summary>
    /// Total number of times this equipment has been used
    /// </summary>
    public int TotalUses { get; set; }
    
    /// <summary>
    /// Average rating for sessions using this equipment
    /// </summary>
    public double AverageRating { get; set; }
    
    /// <summary>
    /// Number of favorite sessions using this equipment
    /// </summary>
    public int FavoriteCount { get; set; }
    
    /// <summary>
    /// Calculated performance score based on usage, ratings, and favorites
    /// </summary>
    public double PerformanceScore { get; set; }
}