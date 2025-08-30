using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public interface IBrewingEquipmentRepository : IRepository<BrewingEquipment>
{
    Task<IEnumerable<BrewingEquipment>> GetByTypeAsync(EquipmentType type);
    Task<IEnumerable<BrewingEquipment>> GetByVendorAsync(string vendor);
    Task<IEnumerable<BrewingEquipment>> GetByModelAsync(string model);
    Task<IEnumerable<BrewingEquipment>> SearchByVendorOrModelAsync(string searchTerm);
    Task<IEnumerable<BrewingEquipment>> GetFilteredAsync(EquipmentType? type = null, string? vendor = null, string? model = null, string? searchTerm = null);
    Task<IEnumerable<BrewingEquipment>> GetMostUsedAsync(int count = 10);
    Task<IEnumerable<string>> GetDistinctVendorsAsync();
    Task<IEnumerable<string>> GetDistinctModelsAsync();
    Task<IEnumerable<BrewingEquipment>> GetBySpecificationAsync(string key, string value);
}