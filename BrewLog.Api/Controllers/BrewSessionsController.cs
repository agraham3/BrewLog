using Microsoft.AspNetCore.Mvc;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Services.Exceptions;
using BrewLog.Api.Models;

namespace BrewLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrewSessionsController : ControllerBase
{
    private readonly IBrewSessionService _brewSessionService;
    private readonly ILogger<BrewSessionsController> _logger;

    public BrewSessionsController(IBrewSessionService brewSessionService, ILogger<BrewSessionsController> logger)
    {
        _brewSessionService = brewSessionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all brew sessions with advanced filtering options
    /// </summary>
    /// <param name="method">Filter by brew method</param>
    /// <param name="coffeeBeanId">Filter by coffee bean ID</param>
    /// <param name="grindSettingId">Filter by grind setting ID</param>
    /// <param name="brewingEquipmentId">Filter by brewing equipment ID</param>
    /// <param name="minWaterTemperature">Filter by minimum water temperature</param>
    /// <param name="maxWaterTemperature">Filter by maximum water temperature</param>
    /// <param name="minRating">Filter by minimum rating</param>
    /// <param name="maxRating">Filter by maximum rating</param>
    /// <param name="isFavorite">Filter by favorite status</param>
    /// <param name="createdAfter">Filter by creation date (after)</param>
    /// <param name="createdBefore">Filter by creation date (before)</param>
    /// <returns>List of brew sessions</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BrewSessionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BrewSessionResponseDto>>> GetBrewSessions(
        [FromQuery] int? method = null,
        [FromQuery] int? coffeeBeanId = null,
        [FromQuery] int? grindSettingId = null,
        [FromQuery] int? brewingEquipmentId = null,
        [FromQuery] decimal? minWaterTemperature = null,
        [FromQuery] decimal? maxWaterTemperature = null,
        [FromQuery] int? minRating = null,
        [FromQuery] int? maxRating = null,
        [FromQuery] bool? isFavorite = null,
        [FromQuery] DateTime? createdAfter = null,
        [FromQuery] DateTime? createdBefore = null)
    {
        _logger.LogInformation("Getting brew sessions with filters: Method={Method}, Bean={Bean}, Rating={MinRating}-{MaxRating}, Favorite={Favorite}", 
            method, coffeeBeanId, minRating, maxRating, isFavorite);

        var filter = new BrewSessionFilterDto
        {
            Method = method.HasValue ? (BrewMethod)method.Value : null,
            CoffeeBeanId = coffeeBeanId,
            GrindSettingId = grindSettingId,
            BrewingEquipmentId = brewingEquipmentId,
            MinWaterTemperature = minWaterTemperature,
            MaxWaterTemperature = maxWaterTemperature,
            MinRating = minRating,
            MaxRating = maxRating,
            IsFavorite = isFavorite,
            CreatedAfter = createdAfter,
            CreatedBefore = createdBefore
        };

        var brewSessions = await _brewSessionService.GetAllAsync(filter);
        return Ok(brewSessions);
    }

    /// <summary>
    /// Get a specific brew session by ID
    /// </summary>
    /// <param name="id">Brew session ID</param>
    /// <returns>Brew session details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BrewSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BrewSessionResponseDto>> GetBrewSession(int id)
    {
        _logger.LogInformation("Getting brew session with ID: {Id}", id);

        var brewSession = await _brewSessionService.GetByIdAsync(id);
        
        if (brewSession == null)
        {
            _logger.LogWarning("Brew session with ID {Id} not found", id);
            return NotFound($"Brew session with ID {id} not found");
        }

        return Ok(brewSession);
    }

    /// <summary>
    /// Create a new brew session
    /// </summary>
    /// <param name="createDto">Brew session creation data</param>
    /// <returns>Created brew session</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BrewSessionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BrewSessionResponseDto>> CreateBrewSession([FromBody] CreateBrewSessionDto createDto)
    {
        _logger.LogInformation("Creating new brew session: Method={Method}, Bean={BeanId}, Rating={Rating}", 
            createDto.Method, createDto.CoffeeBeanId, createDto.Rating);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdSession = await _brewSessionService.CreateAsync(createDto);
            
            _logger.LogInformation("Successfully created brew session with ID: {Id}", createdSession.Id);
            
            return CreatedAtAction(
                nameof(GetBrewSession), 
                new { id = createdSession.Id }, 
                createdSession);
        }
        catch (BusinessValidationException ex)
        {
            _logger.LogWarning("Business validation failed for brew session creation: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing brew session
    /// </summary>
    /// <param name="id">Brew session ID</param>
    /// <param name="updateDto">Brew session update data</param>
    /// <returns>Updated brew session</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BrewSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BrewSessionResponseDto>> UpdateBrewSession(int id, [FromBody] UpdateBrewSessionDto updateDto)
    {
        _logger.LogInformation("Updating brew session with ID: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedSession = await _brewSessionService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Successfully updated brew session with ID: {Id}", id);
            return Ok(updatedSession);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Brew session with ID {Id} not found for update: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (BusinessValidationException ex)
        {
            _logger.LogWarning("Business validation failed for brew session update: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a brew session
    /// </summary>
    /// <param name="id">Brew session ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBrewSession(int id)
    {
        _logger.LogInformation("Deleting brew session with ID: {Id}", id);

        try
        {
            await _brewSessionService.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted brew session with ID: {Id}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Brew session with ID {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Toggle the favorite status of a brew session
    /// </summary>
    /// <param name="id">Brew session ID</param>
    /// <returns>Updated brew session with new favorite status</returns>
    [HttpPost("{id:int}/favorite")]
    [ProducesResponseType(typeof(BrewSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BrewSessionResponseDto>> ToggleFavorite(int id)
    {
        _logger.LogInformation("Toggling favorite status for brew session with ID: {Id}", id);

        try
        {
            var updatedSession = await _brewSessionService.ToggleFavoriteAsync(id);
            _logger.LogInformation("Successfully toggled favorite status for brew session with ID: {Id}. New status: {IsFavorite}", 
                id, updatedSession.IsFavorite);
            return Ok(updatedSession);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Brew session with ID {Id} not found for favorite toggle: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get favorite brew sessions
    /// </summary>
    /// <returns>List of favorite brew sessions</returns>
    [HttpGet("favorites")]
    [ProducesResponseType(typeof(IEnumerable<BrewSessionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BrewSessionResponseDto>>> GetFavorites()
    {
        _logger.LogInformation("Getting favorite brew sessions");

        var favorites = await _brewSessionService.GetFavoritesAsync();
        return Ok(favorites);
    }

    /// <summary>
    /// Get recent brew sessions
    /// </summary>
    /// <param name="count">Number of recent sessions to return (default: 10)</param>
    /// <returns>List of recent brew sessions</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<BrewSessionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BrewSessionResponseDto>>> GetRecent([FromQuery] int count = 10)
    {
        _logger.LogInformation("Getting {Count} recent brew sessions", count);

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var recentSessions = await _brewSessionService.GetRecentAsync(count);
        return Ok(recentSessions);
    }

    /// <summary>
    /// Get top rated brew sessions
    /// </summary>
    /// <param name="count">Number of top rated sessions to return (default: 10)</param>
    /// <returns>List of top rated brew sessions</returns>
    [HttpGet("top-rated")]
    [ProducesResponseType(typeof(IEnumerable<BrewSessionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BrewSessionResponseDto>>> GetTopRated([FromQuery] int count = 10)
    {
        _logger.LogInformation("Getting {Count} top rated brew sessions", count);

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var topRatedSessions = await _brewSessionService.GetTopRatedAsync(count);
        return Ok(topRatedSessions);
    }
}