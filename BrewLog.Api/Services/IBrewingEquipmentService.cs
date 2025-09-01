using BrewLog.Api.DTOs;

namespace BrewLog.Api.Services;

public interface IBrewingEquipmentService
{
    Task<IEnumerable<BrewingEquipmentResponseDto>> GetAllAsync(BrewingEquipmentFilterDto? filter = null);
    Task<BrewingEquipmentResponseDto?> GetByIdAsync(int id);
    Task<BrewingEquipmentResponseDto> CreateAsync(CreateBrewingEquipmentDto dto);
    Task<BrewingEquipmentResponseDto> UpdateAsync(int id, UpdateBrewingEquipmentDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<BrewingEquipmentResponseDto>> GetMostUsedAsync(int count = 10);
    Task<IEnumerable<string>> GetDistinctVendorsAsync();
    Task<IEnumerable<string>> GetDistinctModelsAsync();
    Task<bool> ExistsAsync(int id);
}