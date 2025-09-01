using BrewLog.Api.DTOs;

namespace BrewLog.Api.Services;

public interface ICoffeeBeanService
{
    Task<IEnumerable<CoffeeBeanResponseDto>> GetAllAsync(CoffeeBeanFilterDto? filter = null);
    Task<CoffeeBeanResponseDto?> GetByIdAsync(int id);
    Task<CoffeeBeanResponseDto> CreateAsync(CreateCoffeeBeanDto dto);
    Task<CoffeeBeanResponseDto> UpdateAsync(int id, UpdateCoffeeBeanDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<CoffeeBeanResponseDto>> GetRecentlyAddedAsync(int count = 10);
    Task<IEnumerable<CoffeeBeanResponseDto>> GetMostUsedAsync(int count = 10);
    Task<bool> ExistsAsync(int id);
}