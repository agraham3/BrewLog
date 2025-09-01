using AutoMapper;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using BrewLog.Api.Repositories;
using BrewLog.Api.Services.Exceptions;
using FluentValidation;

namespace BrewLog.Api.Services;

public class CoffeeBeanService(
    ICoffeeBeanRepository repository,
    IBrewSessionRepository brewSessionRepository,
    IMapper mapper,
    IValidator<CreateCoffeeBeanDto> createValidator,
    IValidator<UpdateCoffeeBeanDto> updateValidator) : ICoffeeBeanService
{
    private readonly ICoffeeBeanRepository _repository = repository;
    private readonly IBrewSessionRepository _brewSessionRepository = brewSessionRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<CreateCoffeeBeanDto> _createValidator = createValidator;
    private readonly IValidator<UpdateCoffeeBeanDto> _updateValidator = updateValidator;

    public async Task<IEnumerable<CoffeeBeanResponseDto>> GetAllAsync(CoffeeBeanFilterDto? filter = null)
    {
        try
        {
            IEnumerable<CoffeeBean> beans;

            if (filter == null)
            {
                beans = await _repository.GetAllAsync();
            }
            else
            {
                beans = await _repository.GetFilteredAsync(
                    brand: filter.Brand,
                    roastLevel: filter.RoastLevel,
                    origin: filter.Origin,
                    nameSearch: filter.Name);

                // Apply date filtering if specified
                if (filter.CreatedAfter.HasValue || filter.CreatedBefore.HasValue)
                {
                    beans = beans.Where(b =>
                        (!filter.CreatedAfter.HasValue || b.CreatedDate >= filter.CreatedAfter.Value) &&
                        (!filter.CreatedBefore.HasValue || b.CreatedDate <= filter.CreatedBefore.Value));
                }
            }

            return _mapper.Map<IEnumerable<CoffeeBeanResponseDto>>(beans);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving coffee beans.", ex);
        }
    }

    public async Task<CoffeeBeanResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            var bean = await _repository.GetByIdAsync(id);
            return bean == null ? null : _mapper.Map<CoffeeBeanResponseDto>(bean);
        }
        catch (Exception ex)
        {
            throw new ServiceException($"An error occurred while retrieving coffee bean with ID {id}.", ex);
        }
    }

    public async Task<CoffeeBeanResponseDto> CreateAsync(CreateCoffeeBeanDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            // Check for duplicate name and brand combination
            var existingBeans = await _repository.GetFilteredAsync(brand: dto.Brand, nameSearch: dto.Name);
            if (existingBeans.Any(b => b.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessValidationException($"A coffee bean with name '{dto.Name}' from brand '{dto.Brand}' already exists.");
            }

            var bean = _mapper.Map<CoffeeBean>(dto);
            bean.CreatedDate = DateTime.UtcNow;

            var createdBean = await _repository.AddAsync(bean);
            return _mapper.Map<CoffeeBeanResponseDto>(createdBean);
        }
        catch (BusinessValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while creating the coffee bean.", ex);
        }
    }

    public async Task<CoffeeBeanResponseDto> UpdateAsync(int id, UpdateCoffeeBeanDto dto)
    {
        try
        {
            // Validate input
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessValidationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
            }

            var existingBean = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("CoffeeBean", id);

            // Check for duplicate name and brand combination (excluding current bean)
            var existingBeans = await _repository.GetFilteredAsync(brand: dto.Brand, nameSearch: dto.Name);
            if (existingBeans.Any(b => b.Id != id && b.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessValidationException($"A coffee bean with name '{dto.Name}' from brand '{dto.Brand}' already exists.");
            }

            // Update properties
            existingBean.Name = dto.Name;
            existingBean.Brand = dto.Brand;
            existingBean.RoastLevel = dto.RoastLevel;
            existingBean.Origin = dto.Origin;
            existingBean.ModifiedDate = DateTime.UtcNow;

            var updatedBean = await _repository.UpdateAsync(existingBean);
            return _mapper.Map<CoffeeBeanResponseDto>(updatedBean);
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
            throw new ServiceException($"An error occurred while updating coffee bean with ID {id}.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var existingBean = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("CoffeeBean", id);

            // Check if bean is referenced by any brew sessions
            var brewSessions = await _brewSessionRepository.GetByCoffeeBeanIdAsync(id);
            if (brewSessions.Any())
            {
                throw new ReferentialIntegrityException($"Cannot delete coffee bean '{existingBean.Name}' because it is referenced by {brewSessions.Count()} brew session(s). Please delete the associated brew sessions first.");
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
            throw new ServiceException($"An error occurred while deleting coffee bean with ID {id}.", ex);
        }
    }

    public async Task<IEnumerable<CoffeeBeanResponseDto>> GetRecentlyAddedAsync(int count = 10)
    {
        try
        {
            var beans = await _repository.GetRecentlyAddedAsync(count);
            return _mapper.Map<IEnumerable<CoffeeBeanResponseDto>>(beans);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving recently added coffee beans.", ex);
        }
    }

    public async Task<IEnumerable<CoffeeBeanResponseDto>> GetMostUsedAsync(int count = 10)
    {
        try
        {
            var beans = await _repository.GetMostUsedAsync(count);
            return _mapper.Map<IEnumerable<CoffeeBeanResponseDto>>(beans);
        }
        catch (Exception ex)
        {
            throw new ServiceException("An error occurred while retrieving most used coffee beans.", ex);
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
            throw new ServiceException($"An error occurred while checking if coffee bean with ID {id} exists.", ex);
        }
    }
}