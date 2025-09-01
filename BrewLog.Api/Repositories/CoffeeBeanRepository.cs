using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public class CoffeeBeanRepository(BrewLogDbContext context) : Repository<CoffeeBean>(context), ICoffeeBeanRepository
{
    public async Task<IEnumerable<CoffeeBean>> GetByBrandAsync(string brand)
    {
        return await _dbSet
            .Where(cb => cb.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase))
            .OrderBy(cb => cb.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> GetByRoastLevelAsync(RoastLevel roastLevel)
    {
        return await _dbSet
            .Where(cb => cb.RoastLevel == roastLevel)
            .OrderBy(cb => cb.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> GetByOriginAsync(string origin)
    {
        return await _dbSet
            .Where(cb => cb.Origin.Contains(origin, StringComparison.OrdinalIgnoreCase))
            .OrderBy(cb => cb.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(cb => cb.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .OrderBy(cb => cb.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> GetFilteredAsync(string? brand = null, RoastLevel? roastLevel = null, string? origin = null, string? nameSearch = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(cb => cb.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase));
        }

        if (roastLevel.HasValue)
        {
            query = query.Where(cb => cb.RoastLevel == roastLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(origin))
        {
            query = query.Where(cb => cb.Origin.Contains(origin, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(nameSearch))
        {
            query = query.Where(cb => cb.Name.Contains(nameSearch, StringComparison.OrdinalIgnoreCase));
        }

        return await query
            .OrderBy(cb => cb.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> GetRecentlyAddedAsync(int count = 10)
    {
        return await _dbSet
            .OrderByDescending(cb => cb.CreatedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> GetMostUsedAsync(int count = 10)
    {
        return await _dbSet
            .Include(cb => cb.BrewSessions)
            .OrderByDescending(cb => cb.BrewSessions.Count)
            .Take(count)
            .ToListAsync();
    }
}