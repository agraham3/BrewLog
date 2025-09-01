using BrewLog.Api.DTOs;
using BrewLog.Api.Repositories;

namespace BrewLog.Api.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IBrewSessionRepository _brewSessionRepository;
    private readonly ICoffeeBeanRepository _coffeeBeanRepository;
    private readonly IGrindSettingRepository _grindSettingRepository;
    private readonly IBrewingEquipmentRepository _equipmentRepository;

    public AnalyticsService(
        IBrewSessionRepository brewSessionRepository,
        ICoffeeBeanRepository coffeeBeanRepository,
        IGrindSettingRepository grindSettingRepository,
        IBrewingEquipmentRepository equipmentRepository)
    {
        _brewSessionRepository = brewSessionRepository;
        _coffeeBeanRepository = coffeeBeanRepository;
        _grindSettingRepository = grindSettingRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var brewSessions = await _brewSessionRepository.GetWithIncludesAsync();
        var coffeeBeans = await _coffeeBeanRepository.GetAllAsync();
        var grindSettings = await _grindSettingRepository.GetAllAsync();
        var equipment = await _equipmentRepository.GetAllAsync();

        var brewSessionsList = brewSessions.ToList();
        var ratedSessions = brewSessionsList.Where(bs => bs.Rating.HasValue).ToList();

        // Calculate brew method statistics
        var brewMethodStats = brewSessionsList
            .GroupBy(bs => bs.Method)
            .Select(g => new BrewMethodStatsDto
            {
                Method = g.Key,
                Count = g.Count(),
                AverageRating = g.Where(bs => bs.Rating.HasValue).Average(bs => bs.Rating!.Value),
                FavoriteCount = g.Count(bs => bs.IsFavorite)
            })
            .OrderByDescending(bms => bms.Count)
            .ToList();

        // Calculate equipment statistics
        var equipmentStats = brewSessionsList
            .Where(bs => bs.BrewingEquipmentId.HasValue)
            .GroupBy(bs => new { bs.BrewingEquipmentId, bs.BrewingEquipment })
            .Select(g => new EquipmentStatsDto
            {
                EquipmentId = g.Key.BrewingEquipmentId!.Value,
                EquipmentName = $"{g.Key.BrewingEquipment!.Vendor} {g.Key.BrewingEquipment.Model}",
                Type = g.Key.BrewingEquipment.Type,
                UsageCount = g.Count(),
                AverageRating = g.Where(bs => bs.Rating.HasValue).Any()
                    ? g.Where(bs => bs.Rating.HasValue).Average(bs => bs.Rating!.Value)
                    : 0,
                FavoriteCount = g.Count(bs => bs.IsFavorite)
            })
            .OrderByDescending(es => es.UsageCount)
            .ToList();

        // Get recent brews
        var recentBrews = brewSessionsList
            .OrderByDescending(bs => bs.CreatedDate)
            .Take(5)
            .Select(bs => new RecentBrewSessionDto
            {
                Id = bs.Id,
                Method = bs.Method,
                CoffeeBeanName = $"{bs.CoffeeBean.Brand} {bs.CoffeeBean.Name}",
                Rating = bs.Rating,
                IsFavorite = bs.IsFavorite,
                CreatedDate = bs.CreatedDate
            })
            .ToList();

        return new DashboardStatsDto
        {
            TotalBrewSessions = brewSessionsList.Count,
            TotalCoffeeBeans = coffeeBeans.Count(),
            TotalGrindSettings = grindSettings.Count(),
            TotalEquipment = equipment.Count(),
            FavoriteBrews = brewSessionsList.Count(bs => bs.IsFavorite),
            AverageRating = ratedSessions.Any() ? ratedSessions.Average(bs => bs.Rating!.Value) : 0,
            BrewMethodStats = brewMethodStats,
            EquipmentStats = equipmentStats,
            RecentBrews = recentBrews
        };
    }

    public async Task<CorrelationAnalysisDto> GetCorrelationAnalysisAsync()
    {
        var brewSessions = await _brewSessionRepository.GetWithIncludesAsync();
        var ratedSessions = brewSessions.Where(bs => bs.Rating.HasValue).ToList();

        if (!ratedSessions.Any())
        {
            return new CorrelationAnalysisDto
            {
                GrindSizeCorrelations = [],
                TemperatureCorrelations = [],
                BrewTimeCorrelations = [],
                OverallCorrelationStrength = 0
            };
        }

        // Grind size correlations
        var grindSizeCorrelations = ratedSessions
            .GroupBy(bs => bs.GrindSetting.GrindSize)
            .Where(g => g.Count() >= 2) // Only include grind sizes with at least 2 samples
            .Select(g => new GrindSizeRatingCorrelationDto
            {
                GrindSize = g.Key,
                AverageRating = g.Average(bs => bs.Rating!.Value),
                SampleCount = g.Count()
            })
            .OrderBy(c => c.GrindSize)
            .ToList();

        // Temperature correlations (grouped by 5-degree ranges)
        var temperatureCorrelations = ratedSessions
            .GroupBy(bs => Math.Floor(bs.WaterTemperature / 5) * 5)
            .Where(g => g.Count() >= 2)
            .Select(g => new TemperatureRatingCorrelationDto
            {
                TemperatureRange = (decimal)g.Key,
                AverageRating = g.Average(bs => bs.Rating!.Value),
                SampleCount = g.Count()
            })
            .OrderBy(c => c.TemperatureRange)
            .ToList();

        // Brew time correlations (grouped by 30-second ranges)
        var brewTimeCorrelations = ratedSessions
            .GroupBy(bs => TimeSpan.FromSeconds(Math.Floor(bs.BrewTime.TotalSeconds / 30) * 30))
            .Where(g => g.Count() >= 2)
            .Select(g => new BrewTimeRatingCorrelationDto
            {
                BrewTimeRange = g.Key,
                AverageRating = g.Average(bs => bs.Rating!.Value),
                SampleCount = g.Count()
            })
            .OrderBy(c => c.BrewTimeRange)
            .ToList();

        // Calculate overall correlation strength (simplified metric)
        var overallCorrelationStrength = CalculateOverallCorrelationStrength(
            grindSizeCorrelations, temperatureCorrelations, brewTimeCorrelations);

        return new CorrelationAnalysisDto
        {
            GrindSizeCorrelations = grindSizeCorrelations,
            TemperatureCorrelations = temperatureCorrelations,
            BrewTimeCorrelations = brewTimeCorrelations,
            OverallCorrelationStrength = overallCorrelationStrength
        };
    }

    public async Task<IEnumerable<RecommendationDto>> GetRecommendationsAsync()
    {
        var brewSessions = await _brewSessionRepository.GetWithIncludesAsync();
        var ratedSessions = brewSessions.Where(bs => bs.Rating.HasValue).ToList();
        var recommendations = new List<RecommendationDto>();

        if (!ratedSessions.Any())
        {
            return recommendations;
        }

        // Recommendation 1: Best performing bean
        var bestBean = ratedSessions
            .GroupBy(bs => new { bs.CoffeeBeanId, bs.CoffeeBean })
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Average(bs => bs.Rating!.Value))
            .FirstOrDefault();

        if (bestBean != null)
        {
            recommendations.Add(new RecommendationDto
            {
                Type = "BestBean",
                Title = "Try Your Best Performing Bean",
                Description = $"{bestBean.Key.CoffeeBean.Brand} {bestBean.Key.CoffeeBean.Name} has your highest average rating of {bestBean.Average(bs => bs.Rating!.Value):F1}",
                ConfidenceScore = CalculateConfidenceScore(bestBean.Count(), ratedSessions.Count),
                Parameters = new Dictionary<string, object>
                {
                    ["BeanId"] = bestBean.Key.CoffeeBeanId,
                    ["BeanName"] = $"{bestBean.Key.CoffeeBean.Brand} {bestBean.Key.CoffeeBean.Name}",
                    ["AverageRating"] = bestBean.Average(bs => bs.Rating!.Value)
                }
            });
        }

        // Recommendation 2: Optimal grind size
        var optimalGrindSize = ratedSessions
            .GroupBy(bs => bs.GrindSetting.GrindSize)
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Average(bs => bs.Rating!.Value))
            .FirstOrDefault();

        if (optimalGrindSize != null)
        {
            recommendations.Add(new RecommendationDto
            {
                Type = "OptimalGrindSize",
                Title = "Optimal Grind Size Found",
                Description = $"Grind size {optimalGrindSize.Key} produces your best results with an average rating of {optimalGrindSize.Average(bs => bs.Rating!.Value):F1}",
                ConfidenceScore = CalculateConfidenceScore(optimalGrindSize.Count(), ratedSessions.Count),
                Parameters = new Dictionary<string, object>
                {
                    ["GrindSize"] = optimalGrindSize.Key,
                    ["AverageRating"] = optimalGrindSize.Average(bs => bs.Rating!.Value)
                }
            });
        }

        // Recommendation 3: Best equipment
        var bestEquipment = ratedSessions
            .Where(bs => bs.BrewingEquipmentId.HasValue)
            .GroupBy(bs => new { bs.BrewingEquipmentId, bs.BrewingEquipment })
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Average(bs => bs.Rating!.Value))
            .FirstOrDefault();

        if (bestEquipment != null)
        {
            recommendations.Add(new RecommendationDto
            {
                Type = "BestEquipment",
                Title = "Your Best Performing Equipment",
                Description = $"{bestEquipment.Key.BrewingEquipment!.Vendor} {bestEquipment.Key.BrewingEquipment.Model} gives you the best results with an average rating of {bestEquipment.Average(bs => bs.Rating!.Value):F1}",
                ConfidenceScore = CalculateConfidenceScore(bestEquipment.Count(), ratedSessions.Count),
                Parameters = new Dictionary<string, object>
                {
                    ["EquipmentId"] = bestEquipment.Key.BrewingEquipmentId!.Value,
                    ["EquipmentName"] = $"{bestEquipment.Key.BrewingEquipment.Vendor} {bestEquipment.Key.BrewingEquipment.Model}",
                    ["AverageRating"] = bestEquipment.Average(bs => bs.Rating!.Value)
                }
            });
        }

        // Recommendation 4: Favorite combinations
        var favoriteCombo = ratedSessions
            .Where(bs => bs.IsFavorite)
            .GroupBy(bs => new
            {
                bs.Method,
                BeanName = $"{bs.CoffeeBean.Brand} {bs.CoffeeBean.Name}",
                bs.GrindSetting.GrindSize
            })
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (favoriteCombo != null && favoriteCombo.Count() >= 2)
        {
            recommendations.Add(new RecommendationDto
            {
                Type = "FavoriteCombo",
                Title = "Your Favorite Combination",
                Description = $"You've marked {favoriteCombo.Key.Method} with {favoriteCombo.Key.BeanName} at grind size {favoriteCombo.Key.GrindSize} as favorite {favoriteCombo.Count()} times",
                ConfidenceScore = CalculateConfidenceScore(favoriteCombo.Count(), ratedSessions.Count(bs => bs.IsFavorite)),
                Parameters = new Dictionary<string, object>
                {
                    ["Method"] = favoriteCombo.Key.Method.ToString(),
                    ["BeanName"] = favoriteCombo.Key.BeanName,
                    ["GrindSize"] = favoriteCombo.Key.GrindSize,
                    ["FavoriteCount"] = favoriteCombo.Count()
                }
            });
        }

        return recommendations.OrderByDescending(r => r.ConfidenceScore);
    }

    public async Task<EquipmentPerformanceDto> GetEquipmentPerformanceAsync()
    {
        var brewSessions = await _brewSessionRepository.GetWithIncludesAsync();
        var equipmentSessions = brewSessions
            .Where(bs => bs.BrewingEquipmentId.HasValue)
            .ToList();

        if (!equipmentSessions.Any())
        {
            return new EquipmentPerformanceDto
            {
                EquipmentPerformance = [],
                BestPerformingEquipment = null,
                MostUsedEquipment = null
            };
        }

        var equipmentPerformance = equipmentSessions
            .GroupBy(bs => new { bs.BrewingEquipmentId, bs.BrewingEquipment })
            .Select(g =>
            {
                var ratedSessions = g.Where(bs => bs.Rating.HasValue).ToList();
                var averageRating = ratedSessions.Any() ? ratedSessions.Average(bs => bs.Rating!.Value) : 0;
                var favoriteCount = g.Count(bs => bs.IsFavorite);
                var totalUses = g.Count();

                // Performance score calculation: weighted average of rating, usage, and favorites
                var performanceScore = CalculateEquipmentPerformanceScore(averageRating, totalUses, favoriteCount);

                return new EquipmentPerformanceItemDto
                {
                    EquipmentId = g.Key.BrewingEquipmentId!.Value,
                    Vendor = g.Key.BrewingEquipment!.Vendor,
                    Model = g.Key.BrewingEquipment.Model,
                    Type = g.Key.BrewingEquipment.Type,
                    TotalUses = totalUses,
                    AverageRating = averageRating,
                    FavoriteCount = favoriteCount,
                    PerformanceScore = performanceScore
                };
            })
            .OrderByDescending(ep => ep.PerformanceScore)
            .ToList();

        var bestPerforming = equipmentPerformance
            .Where(ep => ep.AverageRating > 0)
            .OrderByDescending(ep => ep.PerformanceScore)
            .FirstOrDefault();

        var mostUsed = equipmentPerformance
            .OrderByDescending(ep => ep.TotalUses)
            .FirstOrDefault();

        return new EquipmentPerformanceDto
        {
            EquipmentPerformance = equipmentPerformance,
            BestPerformingEquipment = bestPerforming,
            MostUsedEquipment = mostUsed
        };
    }

    private static double CalculateOverallCorrelationStrength(
        List<GrindSizeRatingCorrelationDto> grindSizeCorrelations,
        List<TemperatureRatingCorrelationDto> temperatureCorrelations,
        List<BrewTimeRatingCorrelationDto> brewTimeCorrelations)
    {
        var correlationStrengths = new List<double>();

        // Calculate variance in ratings for each parameter
        if (grindSizeCorrelations.Count > 1)
        {
            var grindRatings = grindSizeCorrelations.Select(c => c.AverageRating).ToList();
            correlationStrengths.Add(CalculateVariance(grindRatings));
        }

        if (temperatureCorrelations.Count > 1)
        {
            var tempRatings = temperatureCorrelations.Select(c => c.AverageRating).ToList();
            correlationStrengths.Add(CalculateVariance(tempRatings));
        }

        if (brewTimeCorrelations.Count > 1)
        {
            var timeRatings = brewTimeCorrelations.Select(c => c.AverageRating).ToList();
            correlationStrengths.Add(CalculateVariance(timeRatings));
        }

        return correlationStrengths.Any() ? correlationStrengths.Average() : 0;
    }

    private static double CalculateVariance(List<double> values)
    {
        if (values.Count < 2) return 0;

        var mean = values.Average();
        var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        return variance;
    }

    private static double CalculateConfidenceScore(int sampleSize, int totalSamples)
    {
        if (totalSamples == 0) return 0;

        // Confidence increases with sample size, but with diminishing returns
        var sampleRatio = (double)sampleSize / totalSamples;
        var confidenceFromSample = Math.Min(sampleSize / 10.0, 1.0); // Max confidence at 10+ samples

        return (sampleRatio * 0.3 + confidenceFromSample * 0.7) * 100;
    }

    private static double CalculateEquipmentPerformanceScore(double averageRating, int totalUses, int favoriteCount)
    {
        // Normalize components to 0-1 scale
        var ratingScore = averageRating / 10.0; // Assuming 10 is max rating
        var usageScore = Math.Min(totalUses / 20.0, 1.0); // Normalize usage (20+ uses = max score)
        var favoriteScore = Math.Min(favoriteCount / 10.0, 1.0); // Normalize favorites (10+ favorites = max score)

        // Weighted combination: rating is most important, then favorites, then usage
        return (ratingScore * 0.5 + favoriteScore * 0.3 + usageScore * 0.2) * 100;
    }
}