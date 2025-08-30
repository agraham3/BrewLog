using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public class BrewSessionRepository : Repository<BrewSession>, IBrewSessionRepository
{
    public BrewSessionRepository(BrewLogDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BrewSession>> GetByBrewMethodAsync(BrewMethod method)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.Method == method)
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetByCoffeeBeanIdAsync(int coffeeBeanId)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.CoffeeBeanId == coffeeBeanId)
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetByGrindSettingIdAsync(int grindSettingId)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.GrindSettingId == grindSettingId)
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetByEquipmentIdAsync(int equipmentId)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.BrewingEquipmentId == equipmentId)
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetFavoritesAsync()
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.IsFavorite)
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetByRatingRangeAsync(int minRating, int maxRating)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.Rating.HasValue && bs.Rating >= minRating && bs.Rating <= maxRating)
            .OrderByDescending(bs => bs.Rating)
            .ThenByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.CreatedDate >= startDate && bs.CreatedDate <= endDate)
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetRecentAsync(int count = 10)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .OrderByDescending(bs => bs.CreatedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetTopRatedAsync(int count = 10)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.Rating.HasValue)
            .OrderByDescending(bs => bs.Rating)
            .ThenByDescending(bs => bs.CreatedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> SearchByTastingNotesAsync(string searchTerm)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .Where(bs => bs.TastingNotes.ToLower().Contains(searchTerm.ToLower()))
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetFilteredAsync(
        BrewMethod? method = null,
        int? coffeeBeanId = null,
        int? grindSettingId = null,
        int? equipmentId = null,
        bool? isFavorite = null,
        int? minRating = null,
        int? maxRating = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? tastingNotesSearch = null)
    {
        var query = _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .AsQueryable();

        if (method.HasValue)
        {
            query = query.Where(bs => bs.Method == method.Value);
        }

        if (coffeeBeanId.HasValue)
        {
            query = query.Where(bs => bs.CoffeeBeanId == coffeeBeanId.Value);
        }

        if (grindSettingId.HasValue)
        {
            query = query.Where(bs => bs.GrindSettingId == grindSettingId.Value);
        }

        if (equipmentId.HasValue)
        {
            query = query.Where(bs => bs.BrewingEquipmentId == equipmentId.Value);
        }

        if (isFavorite.HasValue)
        {
            query = query.Where(bs => bs.IsFavorite == isFavorite.Value);
        }

        if (minRating.HasValue)
        {
            query = query.Where(bs => bs.Rating.HasValue && bs.Rating >= minRating.Value);
        }

        if (maxRating.HasValue)
        {
            query = query.Where(bs => bs.Rating.HasValue && bs.Rating <= maxRating.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(bs => bs.CreatedDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(bs => bs.CreatedDate <= endDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(tastingNotesSearch))
        {
            query = query.Where(bs => bs.TastingNotes.ToLower().Contains(tastingNotesSearch.ToLower()));
        }

        return await query
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewSession>> GetWithIncludesAsync()
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .OrderByDescending(bs => bs.CreatedDate)
            .ToListAsync();
    }

    public async Task<BrewSession?> GetByIdWithIncludesAsync(int id)
    {
        return await _dbSet
            .Include(bs => bs.CoffeeBean)
            .Include(bs => bs.GrindSetting)
            .Include(bs => bs.BrewingEquipment)
            .FirstOrDefaultAsync(bs => bs.Id == id);
    }
}