using AutoMapper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services.Exceptions;
using FluentValidation;

namespace BrewLog.Api.Services;

public class BrewingEquipmentService(
    IBrewingEquipmentRepository repository,
    IBrewSessionRepository brewSessionRepository,
    IMapper mapper,
    IValidator<CreateBrewingEquipmentDto> createValidator,
    IValidator<UpdateBrewingEquipmentDto> updateValidator) : IBrewingEquipmentService
{
    public async Task<IEnumerable<BrewingEquipmentResponseDto>> GetAllAsync(BrewingEquipmentFilterDto? filter = null)
    {
        try
        {
            IEnumerable<BrewingEquipment> equipment;

            if (filter == null)
            {
                equipment = await repository.GetAllAsync();
            }
            else
            {
                equipment = await repository.GetFilteredAsync(
                    type: filter.Type,
                    vendor: filter.Vendor,
                    model: filter.Model);

                // Apply date filtering if specified
                if (filter.CreatedAfter.HasValue || filter.CreatedBefore.HasValue)
                {
                    equipment = equipment.Where(e =>
                        (!filter.CreatedAfter.HasValue || e.CreatedDate >= filter.CreatedAfter.Value) &&
                        (!filter.CreatedBefore.HasValue || e.CreatedDate <= filter.CreatedBefore.Value));
                }
            }

            return mapper.Map<IEnumerable<BrewingEquipmentResponseDto>>(equipment);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving brewing equipment.", ex);
        }
    }

    public async Task<BrewingEquipmentResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            var equipment = await repository.GetByIdAsync(id);
            return equipment is not null ? mapper.Map<BrewingEquipmentResponseDto>(equipment) : null;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while retrieving brewing equipment with ID {id}.", ex);
        }
    }

    public async Task<BrewingEquipmentResponseDto> CreateAsync(CreateBrewingEquipmentDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            // Check for duplicate vendor and model combination
            var existingEquipment = await repository.GetFilteredAsync(vendor: dto.Vendor, model: dto.Model);
            if (existingEquipment.Any())
            {
                throw new BusinessValidationException($"Equipment with vendor '{dto.Vendor}' and model '{dto.Model}' already exists.");
            }

            // Validate specifications based on equipment type
            ValidateSpecifications(dto.Type, dto.Specifications);

            var equipment = mapper.Map<BrewingEquipment>(dto);
            equipment.CreatedDate = DateTime.UtcNow;

            var createdEquipment = await repository.AddAsync(equipment);
            return mapper.Map<BrewingEquipmentResponseDto>(createdEquipment);
        }
        catch (BusinessValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while creating the brewing equipment.", ex);
        }
    }

    public async Task<BrewingEquipmentResponseDto> UpdateAsync(int id, UpdateBrewingEquipmentDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            var existingEquipment = await repository.GetByIdAsync(id) ?? throw new NotFoundException("BrewingEquipment", id);

            // Check for duplicate vendor and model combination (excluding current equipment)
            var duplicateEquipment = await repository.GetFilteredAsync(vendor: dto.Vendor, model: dto.Model);
            if (duplicateEquipment.Any(e => e.Id != id))
            {
                throw new BusinessValidationException($"Equipment with vendor '{dto.Vendor}' and model '{dto.Model}' already exists.");
            }

            // Validate specifications based on equipment type
            ValidateSpecifications(dto.Type, dto.Specifications);

            // Update properties
            existingEquipment.Vendor = dto.Vendor;
            existingEquipment.Model = dto.Model;
            existingEquipment.Type = dto.Type;
            existingEquipment.Specifications = dto.Specifications;

            var updatedEquipment = await repository.UpdateAsync(existingEquipment);
            return mapper.Map<BrewingEquipmentResponseDto>(updatedEquipment);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (BusinessValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while updating brewing equipment with ID {id}.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var existingEquipment = await repository.GetByIdAsync(id) ?? throw new NotFoundException("BrewingEquipment", id);

            // Check if equipment is referenced by any brew sessions
            var brewSessions = await brewSessionRepository.GetByEquipmentIdAsync(id);
            if (brewSessions.Any())
            {
                throw new ReferentialIntegrityException($"Cannot delete equipment '{existingEquipment.Vendor} {existingEquipment.Model}' because it is referenced by {brewSessions.Count()} brew session(s). Please delete the associated brew sessions first.");
            }

            await repository.DeleteAsync(id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (ReferentialIntegrityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while deleting brewing equipment with ID {id}.", ex);
        }
    }

    public async Task<IEnumerable<BrewingEquipmentResponseDto>> GetMostUsedAsync(int count = 10)
    {
        try
        {
            var equipment = await repository.GetMostUsedAsync(count);
            return mapper.Map<IEnumerable<BrewingEquipmentResponseDto>>(equipment);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving most used brewing equipment.", ex);
        }
    }

    public async Task<IEnumerable<string>> GetDistinctVendorsAsync()
    {
        try
        {
            return await repository.GetDistinctVendorsAsync();
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving distinct vendors.", ex);
        }
    }

    public async Task<IEnumerable<string>> GetDistinctModelsAsync()
    {
        try
        {
            return await repository.GetDistinctModelsAsync();
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving distinct models.", ex);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await repository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while checking if brewing equipment with ID {id} exists.", ex);
        }
    }

    private static void ValidateSpecifications(EquipmentType type, Dictionary<string, string> specifications)
    {
        // Business rules for equipment specifications based on type
        switch (type)
        {
            case EquipmentType.EspressoMachine:
                ValidateEspressoMachineSpecs(specifications);
                break;
            case EquipmentType.Grinder:
                ValidateGrinderSpecs(specifications);
                break;
            case EquipmentType.FrenchPress:
                ValidateFrenchPressSpecs(specifications);
                break;
            case EquipmentType.PourOverSetup:
                ValidatePourOverSpecs(specifications);
                break;
            case EquipmentType.DripMachine:
                ValidateDripMachineSpecs(specifications);
                break;
            case EquipmentType.AeroPress:
                ValidateAeroPressSpecs(specifications);
                break;
        }
    }

    private static void ValidateEspressoMachineSpecs(Dictionary<string, string> specs)
    {
        if (specs.TryGetValue("BarPressure", out var barPressure))
        {
            if (!decimal.TryParse(barPressure, out var pressure) || pressure <= 0 || pressure > 20)
            {
                throw new BusinessValidationException("Bar pressure must be a positive number between 0 and 20.");
            }
        }

        if (specs.TryGetValue("BoilerCapacity", out var boilerCapacity))
        {
            if (!decimal.TryParse(boilerCapacity, out var capacity) || capacity <= 0)
            {
                throw new BusinessValidationException("Boiler capacity must be a positive number.");
            }
        }
    }

    private static void ValidateGrinderSpecs(Dictionary<string, string> specs)
    {
        if (specs.TryGetValue("BurrType", out var burrType) && string.IsNullOrWhiteSpace(burrType))
        {
            throw new BusinessValidationException("Burr type cannot be empty.");
        }

        if (specs.TryGetValue("MotorPower", out var motorPower))
        {
            if (!decimal.TryParse(motorPower, out var power) || power <= 0)
            {
                throw new BusinessValidationException("Motor power must be a positive number.");
            }
        }
    }

    private static void ValidateFrenchPressSpecs(Dictionary<string, string> specs)
    {
        if (specs.TryGetValue("Capacity", out var capacity))
        {
            if (!decimal.TryParse(capacity, out var capacityValue) || capacityValue <= 0)
            {
                throw new BusinessValidationException("Capacity must be a positive number.");
            }
        }

        if (specs.TryGetValue("Material", out var material) && string.IsNullOrWhiteSpace(material))
        {
            throw new BusinessValidationException("Material cannot be empty.");
        }
    }

    private static void ValidatePourOverSpecs(Dictionary<string, string> specs)
    {
        if (specs.TryGetValue("FilterType", out var filterType) && string.IsNullOrWhiteSpace(filterType))
        {
            throw new BusinessValidationException("Filter type cannot be empty.");
        }

        if (specs.TryGetValue("Material", out var material) && string.IsNullOrWhiteSpace(material))
        {
            throw new BusinessValidationException("Material cannot be empty.");
        }
    }

    private static void ValidateDripMachineSpecs(Dictionary<string, string> specs)
    {
        if (specs.TryGetValue("Capacity", out var capacity))
        {
            if (!decimal.TryParse(capacity, out var capacityValue) || capacityValue <= 0)
            {
                throw new BusinessValidationException("Capacity must be a positive number.");
            }
        }

        if (specs.TryGetValue("BrewTemperature", out var brewTemperature))
        {
            if (!decimal.TryParse(brewTemperature, out var temp) || temp < 80 || temp > 100)
            {
                throw new BusinessValidationException("Brew temperature must be between 80 and 100 degrees Celsius.");
            }
        }
    }

    private static void ValidateAeroPressSpecs(Dictionary<string, string> specs)
    {
        if (specs.TryGetValue("FilterType", out var filterType) && string.IsNullOrWhiteSpace(filterType))
        {
            throw new BusinessValidationException("Filter type cannot be empty.");
        }

        if (specs.TryGetValue("Material", out var material) && string.IsNullOrWhiteSpace(material))
        {
            throw new BusinessValidationException("Material cannot be empty.");
        }
    }
}