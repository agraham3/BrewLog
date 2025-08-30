using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public class GrindSettingRepository : Repository<GrindSetting>, IGrindSettingRepository
{
    public GrindSettingRepository(BrewLogDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<GrindSetting>> GetByGrinderTypeAsync(string grinderType)
    {
        return await _dbSet
            .Where(gs => gs.GrinderType.ToLower().Contains(grinderType.ToLower()))
            .OrderBy(gs => gs.GrindSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<GrindSetting>> GetByGrindSizeRangeAsync(int minSize, int maxSize)
    {
        return await _dbSet
            .Where(gs => gs.GrindSize >= minSize && gs.GrindSize <= maxSize)
            .OrderBy(gs => gs.GrindSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<GrindSetting>> GetByWeightRangeAsync(decimal minWeight, decimal maxWeight)
    {
        return await _dbSet
            .Where(gs => gs.GrindWeight >= minWeight && gs.GrindWeight <= maxWeight)
            .OrderBy(gs => gs.GrindWeight)
            .ToListAsync();
    }

    public async Task<IEnumerable<GrindSetting>> SearchByNotesAsync(string searchTerm)
    {
        return await _dbSet
            .Where(gs => gs.Notes.ToLower().Contains(searchTerm.ToLower()))
            .OrderBy(gs => gs.GrindSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<GrindSetting>> GetFilteredAsync(string? grinderType = null, int? minGrindSize = null, int? maxGrindSize = null, decimal? minWeight = null, decimal? maxWeight = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(grinderType))
        {
            query = query.Where(gs => gs.GrinderType.ToLower().Contains(grinderType.ToLower()));
        }

        if (minGrindSize.HasValue)
        {
            query = query.Where(gs => gs.GrindSize >= minGrindSize.Value);
        }

        if (maxGrindSize.HasValue)
        {
            query = query.Where(gs => gs.GrindSize <= maxGrindSize.Value);
        }

        if (minWeight.HasValue)
        {
            query = query.Where(gs => gs.GrindWeight >= minWeight.Value);
        }

        if (maxWeight.HasValue)
        {
            query = query.Where(gs => gs.GrindWeight <= maxWeight.Value);
        }

        return await query
            .OrderBy(gs => gs.GrindSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<GrindSetting>> GetRecentlyUsedAsync(int count = 10)
    {
        return await _dbSet
            .Include(gs => gs.BrewSessions)
            .Where(gs => gs.BrewSessions.Any())
            .OrderByDescending(gs => gs.BrewSessions.Max(bs => bs.CreatedDate))
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<GrindSetting>> GetMostUsedAsync(int count = 10)
    {
        return await _dbSet
            .Include(gs => gs.BrewSessions)
            .OrderByDescending(gs => gs.BrewSessions.Count)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetDistinctGrinderTypesAsync()
    {
        return await _dbSet
            .Select(gs => gs.GrinderType)
            .Distinct()
            .OrderBy(gt => gt)
            .ToListAsync();
    }
}