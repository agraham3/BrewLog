using BrewLog.Api.DTOs;

namespace BrewLog.Api.Services;

public interface IBrewSessionService
{
    Task<IEnumerable<BrewSessionResponseDto>> GetAllAsync(BrewSessionFilterDto? filter = null);
    Task<BrewSessionResponseDto?> GetByIdAsync(int id);
    Task<BrewSessionResponseDto> CreateAsync(CreateBrewSessionDto dto);
    Task<BrewSessionResponseDto> UpdateAsync(int id, UpdateBrewSessionDto dto);
    Task DeleteAsync(int id);
    Task<BrewSessionResponseDto> ToggleFavoriteAsync(int id);
    Task<IEnumerable<BrewSessionResponseDto>> GetFavoritesAsync();
    Task<IEnumerable<BrewSessionResponseDto>> GetRecentAsync(int count = 10);
    Task<IEnumerable<BrewSessionResponseDto>> GetTopRatedAsync(int count = 10);
    Task<bool> ExistsAsync(int id);
}