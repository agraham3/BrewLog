using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public interface ICoffeeBeanRepository : IRepository<CoffeeBean>
{
    Task<IEnumerable<CoffeeBean>> GetByBrandAsync(string brand);
    Task<IEnumerable<CoffeeBean>> GetByRoastLevelAsync(RoastLevel roastLevel);
    Task<IEnumerable<CoffeeBean>> GetByOriginAsync(string origin);
    Task<IEnumerable<CoffeeBean>> SearchByNameAsync(string name);
    Task<IEnumerable<CoffeeBean>> GetFilteredAsync(string? brand = null, RoastLevel? roastLevel = null, string? origin = null, string? nameSearch = null);
    Task<IEnumerable<CoffeeBean>> GetRecentlyAddedAsync(int count = 10);
    Task<IEnumerable<CoffeeBean>> GetMostUsedAsync(int count = 10);
}