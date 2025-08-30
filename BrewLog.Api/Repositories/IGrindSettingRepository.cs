using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public interface IGrindSettingRepository : IRepository<GrindSetting>
{
    Task<IEnumerable<GrindSetting>> GetByGrinderTypeAsync(string grinderType);
    Task<IEnumerable<GrindSetting>> GetByGrindSizeRangeAsync(int minSize, int maxSize);
    Task<IEnumerable<GrindSetting>> GetByWeightRangeAsync(decimal minWeight, decimal maxWeight);
    Task<IEnumerable<GrindSetting>> SearchByNotesAsync(string searchTerm);
    Task<IEnumerable<GrindSetting>> GetFilteredAsync(string? grinderType = null, int? minGrindSize = null, int? maxGrindSize = null, decimal? minWeight = null, decimal? maxWeight = null);
    Task<IEnumerable<GrindSetting>> GetRecentlyUsedAsync(int count = 10);
    Task<IEnumerable<GrindSetting>> GetMostUsedAsync(int count = 10);
    Task<IEnumerable<string>> GetDistinctGrinderTypesAsync();
}