using Microsoft.EntityFrameworkCore;
using BrewLog.Api.Data;
using BrewLog.Api.Models;

namespace BrewLog.Api.Repositories;

public class BrewingEquipmentRepository(BrewLogDbContext context) : Repository<BrewingEquipment>(context), IBrewingEquipmentRepository
{

    public async Task<IEnumerable<BrewingEquipment>> GetByTypeAsync(EquipmentType type)
    {
        return await _dbSet
            .Where(be => be.Type == type)
            .OrderBy(be => be.Vendor)
            .ThenBy(be => be.Model)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewingEquipment>> GetByVendorAsync(string vendor)
    {
        return await _dbSet
            .Where(be => be.Vendor.Contains(vendor, StringComparison.OrdinalIgnoreCase))
            .OrderBy(be => be.Vendor)
            .ThenBy(be => be.Model)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewingEquipment>> GetByModelAsync(string model)
    {
        return await _dbSet
            .Where(be => be.Model.Contains(model, StringComparison.OrdinalIgnoreCase))
            .OrderBy(be => be.Vendor)
            .ThenBy(be => be.Model)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewingEquipment>> SearchByVendorOrModelAsync(string searchTerm)
    {
        return await _dbSet
            .Where(be => be.Vendor.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        be.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .OrderBy(be => be.Vendor)
            .ThenBy(be => be.Model)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewingEquipment>> GetFilteredAsync(EquipmentType? type = null, string? vendor = null, string? model = null, string? searchTerm = null)
    {
        var query = _dbSet.AsQueryable();

        if (type.HasValue)
        {
            query = query.Where(be => be.Type == type.Value);
        }

        if (!string.IsNullOrWhiteSpace(vendor))
        {
            query = query.Where(be => be.Vendor.Contains(vendor, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(model))
        {
            query = query.Where(be => be.Model.Contains(model, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(be => be.Vendor.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                     be.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return await query
            .OrderBy(be => be.Vendor)
            .ThenBy(be => be.Model)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewingEquipment>> GetMostUsedAsync(int count = 10)
    {
        return await _dbSet
            .Include(be => be.BrewSessions)
            .OrderByDescending(be => be.BrewSessions.Count)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetDistinctVendorsAsync()
    {
        return await _dbSet
            .Select(be => be.Vendor)
            .Distinct()
            .OrderBy(v => v)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetDistinctModelsAsync()
    {
        return await _dbSet
            .Select(be => be.Model)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();
    }

    public async Task<IEnumerable<BrewingEquipment>> GetBySpecificationAsync(string key, string value)
    {
        // Since JSON queries are complex to translate, we'll use client evaluation
        var allEquipment = await _dbSet.ToListAsync();
        return allEquipment
            .Where(be => be.Specifications.ContainsKey(key) &&
                        be.Specifications[key].Contains(value, StringComparison.OrdinalIgnoreCase))
            .OrderBy(be => be.Vendor)
            .ThenBy(be => be.Model);
    }
}