using AutoMapper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services.Exceptions;
using FluentValidation;

namespace BrewLog.Api.Services;

public class GrindSettingService(
    IGrindSettingRepository repository,
    IBrewSessionRepository brewSessionRepository,
    IMapper mapper,
    IValidator<CreateGrindSettingDto> createValidator,
    IValidator<UpdateGrindSettingDto> updateValidator) : IGrindSettingService
{
    private readonly IGrindSettingRepository _repository = repository;
    private readonly IBrewSessionRepository _brewSessionRepository = brewSessionRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<CreateGrindSettingDto> _createValidator = createValidator;
    private readonly IValidator<UpdateGrindSettingDto> _updateValidator = updateValidator;

    public async Task<IEnumerable<GrindSettingResponseDto>> GetAllAsync(GrindSettingFilterDto? filter = null)
    {
        try
        {
            IEnumerable<GrindSetting> settings;

            if (filter == null)
            {
                settings = await _repository.GetAllAsync();
            }
            else
            {
                settings = await _repository.GetFilteredAsync(
                    grinderType: filter.GrinderType,
                    minGrindSize: filter.MinGrindSize,
                    maxGrindSize: filter.MaxGrindSize,
                    minWeight: filter.MinGrindWeight,
                    maxWeight: filter.MaxGrindWeight);

                // Apply date filtering if specified
                if (filter.CreatedAfter.HasValue || filter.CreatedBefore.HasValue)
                {
                    settings = settings.Where(s =>
                        (!filter.CreatedAfter.HasValue || s.CreatedDate >= filter.CreatedAfter.Value) &&
                        (!filter.CreatedBefore.HasValue || s.CreatedDate <= filter.CreatedBefore.Value));
                }
            }

            return _mapper.Map<IEnumerable<GrindSettingResponseDto>>(settings);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving grind settings.", ex);
        }
    }

    public async Task<GrindSettingResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            var setting = await _repository.GetByIdAsync(id);
            return setting == null ? null : _mapper.Map<GrindSettingResponseDto>(setting);
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while retrieving grind setting with ID {id}.", ex);
        }
    }

    public async Task<GrindSettingResponseDto> CreateAsync(CreateGrindSettingDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            // Business rule: Grind size must be between 1 and 30
            if (dto.GrindSize < 1 || dto.GrindSize > 30)
            {
                throw new BusinessValidationException("Grind size must be between 1 and 30.");
            }

            // Business rule: Grind weight must be positive
            if (dto.GrindWeight <= 0)
            {
                throw new BusinessValidationException("Grind weight must be greater than 0.");
            }

            // Business rule: Grind time must be positive
            if (dto.GrindTime <= TimeSpan.Zero)
            {
                throw new BusinessValidationException("Grind time must be greater than 0.");
            }

            var setting = _mapper.Map<GrindSetting>(dto);
            setting.CreatedDate = DateTime.UtcNow;

            var createdSetting = await _repository.AddAsync(setting);
            return _mapper.Map<GrindSettingResponseDto>(createdSetting);
        }
        catch (BusinessValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while creating the grind setting.", ex);
        }
    }

    public async Task<GrindSettingResponseDto> UpdateAsync(int id, UpdateGrindSettingDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            var existingSetting = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("GrindSetting", id);

            // Business rule: Grind size must be between 1 and 30
            if (dto.GrindSize < 1 || dto.GrindSize > 30)
            {
                throw new BusinessValidationException("Grind size must be between 1 and 30.");
            }

            // Business rule: Grind weight must be positive
            if (dto.GrindWeight <= 0)
            {
                throw new BusinessValidationException("Grind weight must be greater than 0.");
            }

            // Business rule: Grind time must be positive
            if (dto.GrindTime <= TimeSpan.Zero)
            {
                throw new BusinessValidationException("Grind time must be greater than 0.");
            }

            // Update properties
            existingSetting.GrindSize = dto.GrindSize;
            existingSetting.GrindTime = dto.GrindTime;
            existingSetting.GrindWeight = dto.GrindWeight;
            existingSetting.GrinderType = dto.GrinderType;
            existingSetting.Notes = dto.Notes;

            var updatedSetting = await _repository.UpdateAsync(existingSetting);
            return _mapper.Map<GrindSettingResponseDto>(updatedSetting);
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
            throw new ServiceException($"An error occurred while updating grind setting with ID {id}.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var existingSetting = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("GrindSetting", id);

            // Check if setting is referenced by any brew sessions
            var brewSessions = await _brewSessionRepository.GetByGrindSettingIdAsync(id);
            if (brewSessions.Any())
            {
                throw new ReferentialIntegrityException($"Cannot delete grind setting because it is referenced by {brewSessions.Count()} brew session(s). Please delete the associated brew sessions first.");
            }

            await _repository.DeleteAsync(id);
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
            throw new ServiceException($"An error occurred while deleting grind setting with ID {id}.", ex);
        }
    }

    public async Task<IEnumerable<GrindSettingResponseDto>> GetRecentlyUsedAsync(int count = 10)
    {
        try
        {
            var settings = await _repository.GetRecentlyUsedAsync(count);
            return _mapper.Map<IEnumerable<GrindSettingResponseDto>>(settings);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving recently used grind settings.", ex);
        }
    }

    public async Task<IEnumerable<GrindSettingResponseDto>> GetMostUsedAsync(int count = 10)
    {
        try
        {
            var settings = await _repository.GetMostUsedAsync(count);
            return _mapper.Map<IEnumerable<GrindSettingResponseDto>>(settings);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving most used grind settings.", ex);
        }
    }

    public async Task<IEnumerable<string>> GetDistinctGrinderTypesAsync()
    {
        try
        {
            return await _repository.GetDistinctGrinderTypesAsync();
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving distinct grinder types.", ex);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _repository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while checking if grind setting with ID {id} exists.", ex);
        }
    }
}