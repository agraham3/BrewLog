using BrewLog.Api.DTOs;

namespace BrewLog.Api.Services;

public interface IGrindSettingService
{
    Task<IEnumerable<GrindSettingResponseDto>> GetAllAsync(GrindSettingFilterDto? filter = null);
    Task<GrindSettingResponseDto?> GetByIdAsync(int id);
    Task<GrindSettingResponseDto> CreateAsync(CreateGrindSettingDto dto);
    Task<GrindSettingResponseDto> UpdateAsync(int id, UpdateGrindSettingDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<GrindSettingResponseDto>> GetRecentlyUsedAsync(int count = 10);
    Task<IEnumerable<GrindSettingResponseDto>> GetMostUsedAsync(int count = 10);
    Task<IEnumerable<string>> GetDistinctGrinderTypesAsync();
    Task<bool> ExistsAsync(int id);
}