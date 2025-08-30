using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

public class DashboardStatsDto
{
    public int TotalBrewSessions { get; set; }
    public int TotalCoffeeBeans { get; set; }
    public int TotalGrindSettings { get; set; }
    public int TotalEquipment { get; set; }
    public int FavoriteBrews { get; set; }
    public double AverageRating { get; set; }
    public List<BrewMethodStatsDto> BrewMethodStats { get; set; } = [];
    public List<EquipmentStatsDto> EquipmentStats { get; set; } = [];
    public List<RecentBrewSessionDto> RecentBrews { get; set; } = [];
}

public class BrewMethodStatsDto
{
    public BrewMethod Method { get; set; }
    public int Count { get; set; }
    public double AverageRating { get; set; }
    public int FavoriteCount { get; set; }
}

public class EquipmentStatsDto
{
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public int UsageCount { get; set; }
    public double AverageRating { get; set; }
    public int FavoriteCount { get; set; }
}

public class RecentBrewSessionDto
{
    public int Id { get; set; }
    public BrewMethod Method { get; set; }
    public string CoffeeBeanName { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CorrelationAnalysisDto
{
    public List<GrindSizeRatingCorrelationDto> GrindSizeCorrelations { get; set; } = [];
    public List<TemperatureRatingCorrelationDto> TemperatureCorrelations { get; set; } = [];
    public List<BrewTimeRatingCorrelationDto> BrewTimeCorrelations { get; set; } = [];
    public double OverallCorrelationStrength { get; set; }
}

public class GrindSizeRatingCorrelationDto
{
    public int GrindSize { get; set; }
    public double AverageRating { get; set; }
    public int SampleCount { get; set; }
}

public class TemperatureRatingCorrelationDto
{
    public decimal TemperatureRange { get; set; }
    public double AverageRating { get; set; }
    public int SampleCount { get; set; }
}

public class BrewTimeRatingCorrelationDto
{
    public TimeSpan BrewTimeRange { get; set; }
    public double AverageRating { get; set; }
    public int SampleCount { get; set; }
}

public class RecommendationDto
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = [];
}

public class EquipmentPerformanceDto
{
    public List<EquipmentPerformanceItemDto> EquipmentPerformance { get; set; } = [];
    public EquipmentPerformanceItemDto? BestPerformingEquipment { get; set; }
    public EquipmentPerformanceItemDto? MostUsedEquipment { get; set; }
}

public class EquipmentPerformanceItemDto
{
    public int EquipmentId { get; set; }
    public string Vendor { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public int TotalUses { get; set; }
    public double AverageRating { get; set; }
    public int FavoriteCount { get; set; }
    public double PerformanceScore { get; set; }
}