using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public class CoffeeBeanRepository : Repository<CoffeeBean>, ICoffeeBeanRepository
{
    public CoffeeBeanRepository(BrewLogDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CoffeeBean>> GetByBrandAsync(string brand)
    {
        return await _dbSet
            .Where(cb => cb.Brand.ToLower().Contains(brand.ToLower()))
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
            .Where(cb => cb.Origin.ToLower().Contains(origin.ToLower()))
            .OrderBy(cb => cb.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(cb => cb.Name.ToLower().Contains(name.ToLower()))
            .OrderBy(cb => cb.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CoffeeBean>> GetFilteredAsync(string? brand = null, RoastLevel? roastLevel = null, string? origin = null, string? nameSearch = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(cb => cb.Brand.ToLower().Contains(brand.ToLower()));
        }

        if (roastLevel.HasValue)
        {
            query = query.Where(cb => cb.RoastLevel == roastLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(origin))
        {
            query = query.Where(cb => cb.Origin.ToLower().Contains(origin.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(nameSearch))
        {
            query = query.Where(cb => cb.Name.ToLower().Contains(nameSearch.ToLower()));
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