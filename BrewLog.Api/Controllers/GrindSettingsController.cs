using Microsoft.AspNetCore.Mvc;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Services.Exceptions;

namespace BrewLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrindSettingsController : ControllerBase
{
    private readonly IGrindSettingService _grindSettingService;
    private readonly ILogger<GrindSettingsController> _logger;

    public GrindSettingsController(IGrindSettingService grindSettingService, ILogger<GrindSettingsController> logger)
    {
        _grindSettingService = grindSettingService;
        _logger = logger;
    }

    /// <summary>
    /// Get all grind settings with optional filtering
    /// </summary>
    /// <param name="minGrindSize">Filter by minimum grind size on a 1-30 scale where 1=finest (espresso) and 30=coarsest (cold brew). Settings with grind size >= this value will be returned.</param>
    /// <param name="maxGrindSize">Filter by maximum grind size on a 1-30 scale where 1=finest (espresso) and 30=coarsest (cold brew). Settings with grind size <= this value will be returned.</param>
    /// <param name="grinderType">Filter by grinder type using partial case-insensitive matching. Example: "burr" will match "Burr Grinder" and "Conical Burr"</param>
    /// <param name="minGrindWeight">Filter by minimum grind weight in grams. Settings with weight >= this value will be returned. Typical range: 10-50 grams.</param>
    /// <param name="maxGrindWeight">Filter by maximum grind weight in grams. Settings with weight <= this value will be returned. Typical range: 10-50 grams.</param>
    /// <param name="createdAfter">Filter by creation date (inclusive). Only settings created on or after this date will be returned. Format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS</param>
    /// <param name="createdBefore">Filter by creation date (inclusive). Only settings created on or before this date will be returned. Format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS</param>
    /// <returns>List of grind settings matching the specified filters, ordered by creation date descending</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GrindSettingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GrindSettingResponseDto>>> GetGrindSettings(
        [FromQuery] int? minGrindSize = null,
        [FromQuery] int? maxGrindSize = null,
        [FromQuery] string? grinderType = null,
        [FromQuery] decimal? minGrindWeight = null,
        [FromQuery] decimal? maxGrindWeight = null,
        [FromQuery] DateTime? createdAfter = null,
        [FromQuery] DateTime? createdBefore = null)
    {
        _logger.LogInformation("Getting grind settings with filters: MinGrindSize={MinGrindSize}, MaxGrindSize={MaxGrindSize}, GrinderType={GrinderType}", 
            minGrindSize, maxGrindSize, grinderType);

        var filter = new GrindSettingFilterDto
        {
            MinGrindSize = minGrindSize,
            MaxGrindSize = maxGrindSize,
            GrinderType = grinderType,
            MinGrindWeight = minGrindWeight,
            MaxGrindWeight = maxGrindWeight,
            CreatedAfter = createdAfter,
            CreatedBefore = createdBefore
        };

        var grindSettings = await _grindSettingService.GetAllAsync(filter);
        return Ok(grindSettings);
    }

    /// <summary>
    /// Get a specific grind setting by ID
    /// </summary>
    /// <param name="id">The unique identifier of the grind setting to retrieve. Must be a positive integer.</param>
    /// <returns>Grind setting details including grind size, weight, grinder type, and usage statistics</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GrindSettingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GrindSettingResponseDto>> GetGrindSetting(int id)
    {
        _logger.LogInformation("Getting grind setting with ID: {Id}", id);

        var grindSetting = await _grindSettingService.GetByIdAsync(id);
        
        if (grindSetting == null)
        {
            _logger.LogWarning("Grind setting with ID {Id} not found", id);
            return NotFound($"Grind setting with ID {id} not found");
        }

        return Ok(grindSetting);
    }

    /// <summary>
    /// Create a new grind setting
    /// </summary>
    /// <param name="createDto">Grind setting creation data</param>
    /// <returns>Created grind setting</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GrindSettingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GrindSettingResponseDto>> CreateGrindSetting([FromBody] CreateGrindSettingDto createDto)
    {
        _logger.LogInformation("Creating new grind setting: Size={GrindSize}, Type={GrinderType}", createDto.GrindSize, createDto.GrinderType);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdSetting = await _grindSettingService.CreateAsync(createDto);
        
        _logger.LogInformation("Successfully created grind setting with ID: {Id}", createdSetting.Id);
        
        return CreatedAtAction(
            nameof(GetGrindSetting), 
            new { id = createdSetting.Id }, 
            createdSetting);
    }

    /// <summary>
    /// Update an existing grind setting
    /// </summary>
    /// <param name="id">Grind setting ID</param>
    /// <param name="updateDto">Grind setting update data</param>
    /// <returns>Updated grind setting</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GrindSettingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GrindSettingResponseDto>> UpdateGrindSetting(int id, [FromBody] UpdateGrindSettingDto updateDto)
    {
        _logger.LogInformation("Updating grind setting with ID: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedSetting = await _grindSettingService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Successfully updated grind setting with ID: {Id}", id);
            return Ok(updatedSetting);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Grind setting with ID {Id} not found for update: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete a grind setting
    /// </summary>
    /// <param name="id">Grind setting ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteGrindSetting(int id)
    {
        _logger.LogInformation("Deleting grind setting with ID: {Id}", id);

        try
        {
            await _grindSettingService.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted grind setting with ID: {Id}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Grind setting with ID {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Cannot delete grind setting with ID {Id}: {Message}", id, ex.Message);
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Get recently used grind settings ordered by last usage date (most recent first)
    /// </summary>
    /// <param name="count">Number of recent settings to return. Must be between 1 and 100. Default: 10</param>
    /// <returns>List of recently used grind settings ordered by last usage date descending</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<GrindSettingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GrindSettingResponseDto>>> GetRecentGrindSettings([FromQuery] int count = 10)
    {
        _logger.LogInformation("Getting {Count} recently used grind settings", count);

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var recentSettings = await _grindSettingService.GetRecentlyUsedAsync(count);
        return Ok(recentSettings);
    }

    /// <summary>
    /// Get most used grind settings ordered by usage frequency (most used first)
    /// </summary>
    /// <param name="count">Number of most used settings to return. Must be between 1 and 100. Default: 10</param>
    /// <returns>List of most used grind settings ordered by usage frequency descending</returns>
    [HttpGet("most-used")]
    [ProducesResponseType(typeof(IEnumerable<GrindSettingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GrindSettingResponseDto>>> GetMostUsedGrindSettings([FromQuery] int count = 10)
    {
        _logger.LogInformation("Getting {Count} most used grind settings", count);

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var mostUsedSettings = await _grindSettingService.GetMostUsedAsync(count);
        return Ok(mostUsedSettings);
    }

    /// <summary>
    /// Get distinct grinder types
    /// </summary>
    /// <returns>List of distinct grinder types</returns>
    [HttpGet("grinder-types")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<string>>> GetGrinderTypes()
    {
        _logger.LogInformation("Getting distinct grinder types");

        var grinderTypes = await _grindSettingService.GetDistinctGrinderTypesAsync();
        return Ok(grinderTypes);
    }
}