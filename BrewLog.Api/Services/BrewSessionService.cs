using AutoMapper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services.Exceptions;
using FluentValidation;

namespace BrewLog.Api.Services;

public class BrewSessionService(
    IBrewSessionRepository repository,
    ICoffeeBeanRepository coffeeBeanRepository,
    IGrindSettingRepository grindSettingRepository,
    IBrewingEquipmentRepository equipmentRepository,
    IMapper mapper,
    IValidator<CreateBrewSessionDto> createValidator,
    IValidator<UpdateBrewSessionDto> updateValidator) : IBrewSessionService
{
    private readonly IBrewSessionRepository _repository = repository;
    private readonly ICoffeeBeanRepository _coffeeBeanRepository = coffeeBeanRepository;
    private readonly IGrindSettingRepository _grindSettingRepository = grindSettingRepository;
    private readonly IBrewingEquipmentRepository _equipmentRepository = equipmentRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<CreateBrewSessionDto> _createValidator = createValidator;
    private readonly IValidator<UpdateBrewSessionDto> _updateValidator = updateValidator;

    public async Task<IEnumerable<BrewSessionResponseDto>> GetAllAsync(BrewSessionFilterDto? filter = null)
    {
        try
        {
            IEnumerable<BrewSession> sessions;

            if (filter == null)
            {
                sessions = await _repository.GetWithIncludesAsync();
            }
            else
            {
                sessions = await _repository.GetFilteredAsync(
                    method: filter.Method,
                    coffeeBeanId: filter.CoffeeBeanId,
                    grindSettingId: filter.GrindSettingId,
                    equipmentId: filter.BrewingEquipmentId,
                    isFavorite: filter.IsFavorite,
                    minRating: filter.MinRating,
                    maxRating: filter.MaxRating,
                    startDate: filter.CreatedAfter,
                    endDate: filter.CreatedBefore);

                // Apply temperature filtering if specified
                if (filter.MinWaterTemperature.HasValue || filter.MaxWaterTemperature.HasValue)
                {
                    sessions = sessions.Where(s =>
                        (!filter.MinWaterTemperature.HasValue || s.WaterTemperature >= filter.MinWaterTemperature.Value) &&
                        (!filter.MaxWaterTemperature.HasValue || s.WaterTemperature <= filter.MaxWaterTemperature.Value));
                }
            }

            return _mapper.Map<IEnumerable<BrewSessionResponseDto>>(sessions);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving brew sessions.", ex);
        }
    }

    public async Task<BrewSessionResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            var session = await _repository.GetByIdWithIncludesAsync(id);
            return session == null ? null : _mapper.Map<BrewSessionResponseDto>(session);
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while retrieving brew session with ID {id}.", ex);
        }
    }

    public async Task<BrewSessionResponseDto> CreateAsync(CreateBrewSessionDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            // Validate foreign key references
            await ValidateForeignKeyReferences(dto.CoffeeBeanId, dto.GrindSettingId, dto.BrewingEquipmentId);

            // Business rules validation
            ValidateBrewSessionBusinessRules(dto.Method, dto.WaterTemperature, dto.BrewTime, dto.Rating);

            var session = _mapper.Map<BrewSession>(dto);
            session.CreatedDate = DateTime.UtcNow;

            var createdSession = await _repository.AddAsync(session);

            // Fetch the created session with includes for proper response
            var sessionWithIncludes = await _repository.GetByIdWithIncludesAsync(createdSession.Id);
            return _mapper.Map<BrewSessionResponseDto>(sessionWithIncludes!);
        }
        catch (BusinessValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while creating the brew session.", ex);
        }
    }

    public async Task<BrewSessionResponseDto> UpdateAsync(int id, UpdateBrewSessionDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            var existingSession = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("BrewSession", id);

            // Validate foreign key references
            await ValidateForeignKeyReferences(dto.CoffeeBeanId, dto.GrindSettingId, dto.BrewingEquipmentId);

            // Business rules validation
            ValidateBrewSessionBusinessRules(dto.Method, dto.WaterTemperature, dto.BrewTime, dto.Rating);

            // Update properties
            existingSession.Method = dto.Method;
            existingSession.WaterTemperature = dto.WaterTemperature;
            existingSession.BrewTime = dto.BrewTime;
            existingSession.TastingNotes = dto.TastingNotes;
            existingSession.Rating = dto.Rating;
            existingSession.IsFavorite = dto.IsFavorite;
            existingSession.CoffeeBeanId = dto.CoffeeBeanId;
            existingSession.GrindSettingId = dto.GrindSettingId;
            existingSession.BrewingEquipmentId = dto.BrewingEquipmentId;

            var updatedSession = await _repository.UpdateAsync(existingSession);

            // Fetch the updated session with includes for proper response
            var sessionWithIncludes = await _repository.GetByIdWithIncludesAsync(updatedSession.Id);
            return _mapper.Map<BrewSessionResponseDto>(sessionWithIncludes!);
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
            throw new ServiceException($"An error occurred while updating brew session with ID {id}.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var existingSession = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("BrewSession", id);
            await _repository.DeleteAsync(id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while deleting brew session with ID {id}.", ex);
        }
    }

    public async Task<BrewSessionResponseDto> ToggleFavoriteAsync(int id)
    {
        try
        {
            var existingSession = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("BrewSession", id);

            // Toggle the favorite status
            existingSession.IsFavorite = !existingSession.IsFavorite;

            var updatedSession = await _repository.UpdateAsync(existingSession);

            // Fetch the updated session with includes for proper response
            var sessionWithIncludes = await _repository.GetByIdWithIncludesAsync(updatedSession.Id);
            return _mapper.Map<BrewSessionResponseDto>(sessionWithIncludes!);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while toggling favorite status for brew session with ID {id}.", ex);
        }
    }

    public async Task<IEnumerable<BrewSessionResponseDto>> GetFavoritesAsync()
    {
        try
        {
            var sessions = await _repository.GetFavoritesAsync();
            return _mapper.Map<IEnumerable<BrewSessionResponseDto>>(sessions);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving favorite brew sessions.", ex);
        }
    }

    public async Task<IEnumerable<BrewSessionResponseDto>> GetRecentAsync(int count = 10)
    {
        try
        {
            var sessions = await _repository.GetRecentAsync(count);
            return _mapper.Map<IEnumerable<BrewSessionResponseDto>>(sessions);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving recent brew sessions.", ex);
        }
    }

    public async Task<IEnumerable<BrewSessionResponseDto>> GetTopRatedAsync(int count = 10)
    {
        try
        {
            var sessions = await _repository.GetTopRatedAsync(count);
            return _mapper.Map<IEnumerable<BrewSessionResponseDto>>(sessions);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving top rated brew sessions.", ex);
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
            throw new ServiceException($"An error occurred while checking if brew session with ID {id} exists.", ex);
        }
    }

    private async Task ValidateForeignKeyReferences(int coffeeBeanId, int grindSettingId, int? equipmentId)
    {
        // Validate coffee bean exists
        if (!await _coffeeBeanRepository.ExistsAsync(coffeeBeanId))
        {
            throw new BusinessValidationException($"Coffee bean with ID {coffeeBeanId} does not exist.");
        }

        // Validate grind setting exists
        if (!await _grindSettingRepository.ExistsAsync(grindSettingId))
        {
            throw new BusinessValidationException($"Grind setting with ID {grindSettingId} does not exist.");
        }

        // Validate equipment exists (if provided)
        if (equipmentId.HasValue && !await _equipmentRepository.ExistsAsync(equipmentId.Value))
        {
            throw new BusinessValidationException($"Brewing equipment with ID {equipmentId.Value} does not exist.");
        }
    }

    private static void ValidateBrewSessionBusinessRules(BrewMethod method, decimal waterTemperature, TimeSpan brewTime, int? rating)
    {
        // Validate water temperature ranges based on brew method
        var (minTemp, maxTemp) = GetTemperatureRangeForMethod(method);
        if (waterTemperature < minTemp || waterTemperature > maxTemp)
        {
            throw new BusinessValidationException($"Water temperature for {method} should be between {minTemp}°C and {maxTemp}°C.");
        }

        // Validate brew time is positive
        if (brewTime <= TimeSpan.Zero)
        {
            throw new BusinessValidationException("Brew time must be greater than 0.");
        }

        // Validate brew time ranges based on method
        var (minTime, maxTime) = GetTimeRangeForMethod(method);
        if (brewTime < minTime || brewTime > maxTime)
        {
            throw new BusinessValidationException($"Brew time for {method} should be between {minTime.TotalMinutes:F1} and {maxTime.TotalMinutes:F1} minutes.");
        }

        // Validate rating if provided
        if (rating.HasValue && (rating.Value < 1 || rating.Value > 10))
        {
            throw new BusinessValidationException("Rating must be between 1 and 10.");
        }
    }

    private static (decimal minTemp, decimal maxTemp) GetTemperatureRangeForMethod(BrewMethod method)
    {
        return method switch
        {
            BrewMethod.Espresso => (88m, 96m),
            BrewMethod.FrenchPress => (92m, 96m),
            BrewMethod.PourOver => (90m, 96m),
            BrewMethod.Drip => (90m, 96m),
            BrewMethod.AeroPress => (80m, 95m),
            BrewMethod.ColdBrew => (4m, 25m), // Cold brew uses cold water
            _ => (80m, 100m) // Default range
        };
    }

    private static (TimeSpan minTime, TimeSpan maxTime) GetTimeRangeForMethod(BrewMethod method)
    {
        return method switch
        {
            BrewMethod.Espresso => (TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(40)),
            BrewMethod.FrenchPress => (TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5)),
            BrewMethod.PourOver => (TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(6)),
            BrewMethod.Drip => (TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(8)),
            BrewMethod.AeroPress => (TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(3)),
            BrewMethod.ColdBrew => (TimeSpan.FromHours(8), TimeSpan.FromHours(24)),
            _ => (TimeSpan.FromSeconds(30), TimeSpan.FromHours(24)) // Default range
        };
    }
}