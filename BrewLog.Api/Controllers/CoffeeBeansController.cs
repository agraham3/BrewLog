using Microsoft.AspNetCore.Mvc;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Services.Exceptions;

namespace BrewLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoffeeBeansController : ControllerBase
{
    private readonly ICoffeeBeanService _coffeeBeanService;
    private readonly ILogger<CoffeeBeansController> _logger;

    public CoffeeBeansController(ICoffeeBeanService coffeeBeanService, ILogger<CoffeeBeansController> logger)
    {
        _coffeeBeanService = coffeeBeanService;
        _logger = logger;
    }

    /// <summary>
    /// Get all coffee beans with optional filtering
    /// </summary>
    /// <param name="name">Filter by coffee bean name using partial case-insensitive matching. Example: "ethiopian" will match "Ethiopian Yirgacheffe"</param>
    /// <param name="brand">Filter by brand name using partial case-insensitive matching. Example: "blue" will match "Blue Bottle Coffee"</param>
    /// <param name="roastLevel">Filter by roast level using integer values: 0=Light, 1=MediumLight, 2=Medium, 3=MediumDark, 4=Dark. String values also accepted: "Light", "Medium", etc.</param>
    /// <param name="origin">Filter by origin/region using partial case-insensitive matching. Example: "ethiopia" will match "Ethiopia, Sidamo"</param>
    /// <param name="createdAfter">Filter by creation date (inclusive). Only beans created on or after this date will be returned. Format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS</param>
    /// <param name="createdBefore">Filter by creation date (inclusive). Only beans created on or before this date will be returned. Format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS</param>
    /// <returns>List of coffee beans matching the specified filters</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CoffeeBeanResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CoffeeBeanResponseDto>>> GetCoffeeBeans(
        [FromQuery] string? name = null,
        [FromQuery] string? brand = null,
        [FromQuery] int? roastLevel = null,
        [FromQuery] string? origin = null,
        [FromQuery] DateTime? createdAfter = null,
        [FromQuery] DateTime? createdBefore = null)
    {
        _logger.LogInformation("Getting coffee beans with filters: Name={Name}, Brand={Brand}, RoastLevel={RoastLevel}, Origin={Origin}", 
            name, brand, roastLevel, origin);

        var filter = new CoffeeBeanFilterDto
        {
            Name = name,
            Brand = brand,
            RoastLevel = roastLevel.HasValue ? (Models.RoastLevel)roastLevel.Value : null,
            Origin = origin,
            CreatedAfter = createdAfter,
            CreatedBefore = createdBefore
        };

        var coffeeBeans = await _coffeeBeanService.GetAllAsync(filter);
        return Ok(coffeeBeans);
    }

    /// <summary>
    /// Get a specific coffee bean by ID
    /// </summary>
    /// <param name="id">The unique identifier of the coffee bean to retrieve. Must be a positive integer.</param>
    /// <returns>Coffee bean details including all properties and relationships</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CoffeeBeanResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CoffeeBeanResponseDto>> GetCoffeeBean(int id)
    {
        _logger.LogInformation("Getting coffee bean with ID: {Id}", id);

        var coffeeBean = await _coffeeBeanService.GetByIdAsync(id);
        
        if (coffeeBean == null)
        {
            _logger.LogWarning("Coffee bean with ID {Id} not found", id);
            return NotFound($"Coffee bean with ID {id} not found");
        }

        return Ok(coffeeBean);
    }

    /// <summary>
    /// Create a new coffee bean
    /// </summary>
    /// <param name="createDto">Coffee bean creation data</param>
    /// <returns>Created coffee bean</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CoffeeBeanResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CoffeeBeanResponseDto>> CreateCoffeeBean([FromBody] CreateCoffeeBeanDto createDto)
    {
        _logger.LogInformation("Creating new coffee bean: {Name} by {Brand}", createDto.Name, createDto.Brand);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdBean = await _coffeeBeanService.CreateAsync(createDto);
        
        _logger.LogInformation("Successfully created coffee bean with ID: {Id}", createdBean.Id);
        
        return CreatedAtAction(
            nameof(GetCoffeeBean), 
            new { id = createdBean.Id }, 
            createdBean);
    }

    /// <summary>
    /// Update an existing coffee bean
    /// </summary>
    /// <param name="id">Coffee bean ID</param>
    /// <param name="updateDto">Coffee bean update data</param>
    /// <returns>Updated coffee bean</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CoffeeBeanResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CoffeeBeanResponseDto>> UpdateCoffeeBean(int id, [FromBody] UpdateCoffeeBeanDto updateDto)
    {
        _logger.LogInformation("Updating coffee bean with ID: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedBean = await _coffeeBeanService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Successfully updated coffee bean with ID: {Id}", id);
            return Ok(updatedBean);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Coffee bean with ID {Id} not found for update: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete a coffee bean
    /// </summary>
    /// <param name="id">Coffee bean ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCoffeeBean(int id)
    {
        _logger.LogInformation("Deleting coffee bean with ID: {Id}", id);

        try
        {
            await _coffeeBeanService.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted coffee bean with ID: {Id}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Coffee bean with ID {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Cannot delete coffee bean with ID {Id}: {Message}", id, ex.Message);
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Get recently added coffee beans ordered by creation date (newest first)
    /// </summary>
    /// <param name="count">Number of recent beans to return. Must be between 1 and 100. Default: 10</param>
    /// <returns>List of recently added coffee beans ordered by creation date descending</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<CoffeeBeanResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CoffeeBeanResponseDto>>> GetRecentCoffeeBeans([FromQuery] int count = 10)
    {
        _logger.LogInformation("Getting {Count} recently added coffee beans", count);

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var recentBeans = await _coffeeBeanService.GetRecentlyAddedAsync(count);
        return Ok(recentBeans);
    }

    /// <summary>
    /// Get most used coffee beans ordered by usage frequency (most used first)
    /// </summary>
    /// <param name="count">Number of most used beans to return. Must be between 1 and 100. Default: 10</param>
    /// <returns>List of most used coffee beans ordered by usage frequency descending</returns>
    [HttpGet("most-used")]
    [ProducesResponseType(typeof(IEnumerable<CoffeeBeanResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CoffeeBeanResponseDto>>> GetMostUsedCoffeeBeans([FromQuery] int count = 10)
    {
        _logger.LogInformation("Getting {Count} most used coffee beans", count);

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var mostUsedBeans = await _coffeeBeanService.GetMostUsedAsync(count);
        return Ok(mostUsedBeans);
    }
}