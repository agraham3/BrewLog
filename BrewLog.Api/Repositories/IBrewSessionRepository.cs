using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public interface IBrewSessionRepository : IRepository<BrewSession>
{
    Task<IEnumerable<BrewSession>> GetByBrewMethodAsync(BrewMethod method);
    Task<IEnumerable<BrewSession>> GetByCoffeeBeanIdAsync(int coffeeBeanId);
    Task<IEnumerable<BrewSession>> GetByGrindSettingIdAsync(int grindSettingId);
    Task<IEnumerable<BrewSession>> GetByEquipmentIdAsync(int equipmentId);
    Task<IEnumerable<BrewSession>> GetFavoritesAsync();
    Task<IEnumerable<BrewSession>> GetByRatingRangeAsync(int minRating, int maxRating);
    Task<IEnumerable<BrewSession>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<BrewSession>> GetRecentAsync(int count = 10);
    Task<IEnumerable<BrewSession>> GetTopRatedAsync(int count = 10);
    Task<IEnumerable<BrewSession>> SearchByTastingNotesAsync(string searchTerm);
    Task<IEnumerable<BrewSession>> GetFilteredAsync(
        BrewMethod? method = null,
        int? coffeeBeanId = null,
        int? grindSettingId = null,
        int? equipmentId = null,
        bool? isFavorite = null,
        int? minRating = null,
        int? maxRating = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? tastingNotesSearch = null);
    Task<IEnumerable<BrewSession>> GetWithIncludesAsync();
    Task<BrewSession?> GetByIdWithIncludesAsync(int id);
}