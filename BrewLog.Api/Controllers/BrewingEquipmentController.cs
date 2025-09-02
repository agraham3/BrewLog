using Microsoft.AspNetCore.Mvc;
using BrewLog.Api.Services;
using BrewLog.Api.DTOs;
using BrewLog.Api.Services.Exceptions;
using BrewLog.Api.Models;
using BrewLog.Api.Attributes;

namespace BrewLog.Api.Controllers;

[ApiController]
[Route("api/equipment")]
public class BrewingEquipmentController : ControllerBase
{
    private readonly IBrewingEquipmentService _brewingEquipmentService;
    private readonly ILogger<BrewingEquipmentController> _logger;

    public BrewingEquipmentController(IBrewingEquipmentService brewingEquipmentService, ILogger<BrewingEquipmentController> logger)
    {
        _brewingEquipmentService = brewingEquipmentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all brewing equipment with optional filtering by type and vendor
    /// </summary>
    /// <param name="type">Filter by equipment type using integer values: 0=EspressoMachine, 1=Grinder, 2=FrenchPress, 3=PourOverSetup, 4=DripMachine, 5=AeroPress. String values also accepted: "EspressoMachine", "Grinder", etc.</param>
    /// <param name="vendor">Filter by vendor name using partial case-insensitive matching. Example: "breville" will match "Breville" and "Breville USA"</param>
    /// <param name="model">Filter by model name using partial case-insensitive matching. Example: "barista" will match "Barista Express" and "Barista Pro"</param>
    /// <param name="createdAfter">Filter by creation date (inclusive). Only equipment created on or after this date will be returned. Format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS</param>
    /// <param name="createdBefore">Filter by creation date (inclusive). Only equipment created on or before this date will be returned. Format: YYYY-MM-DD or YYYY-MM-DDTHH:MM:SS</param>
    /// <returns>List of brewing equipment matching the specified filters, ordered by creation date descending</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BrewingEquipmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "BrewingEquipmentCollection", "Collection of brewing equipment with filtering applied")]
    [SwaggerResponseExample(200, "BrewingEquipmentEmpty", "Empty collection when no equipment matches filters")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<IEnumerable<BrewingEquipmentResponseDto>>> GetBrewingEquipment(
        [FromQuery] EquipmentType? type = null,
        [FromQuery] string? vendor = null,
        [FromQuery] string? model = null,
        [FromQuery] DateTime? createdAfter = null,
        [FromQuery] DateTime? createdBefore = null)
    {
        _logger.LogInformation("Getting brewing equipment with filters: Type={Type}, Vendor={Vendor}, Model={Model}", 
            type, vendor, model);

        var filter = new BrewingEquipmentFilterDto
        {
            Type = type,
            Vendor = vendor,
            Model = model,
            CreatedAfter = createdAfter,
            CreatedBefore = createdBefore
        };

        var equipment = await _brewingEquipmentService.GetAllAsync(filter);
        return Ok(equipment);
    }

    /// <summary>
    /// Get a specific brewing equipment by ID
    /// </summary>
    /// <param name="id">The unique identifier of the brewing equipment to retrieve. Must be a positive integer.</param>
    /// <returns>Brewing equipment details including specifications, usage statistics, and all properties</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BrewingEquipmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "BrewingEquipmentSuccess", "Successfully retrieved brewing equipment")]
    [SwaggerResponseExample(404, "NotFoundError", "Brewing equipment not found")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<BrewingEquipmentResponseDto>> GetBrewingEquipment(int id)
    {
        _logger.LogInformation("Getting brewing equipment with ID: {Id}", id);

        var equipment = await _brewingEquipmentService.GetByIdAsync(id);
        
        if (equipment == null)
        {
            _logger.LogWarning("Brewing equipment with ID {Id} not found", id);
            return NotFound($"Brewing equipment with ID {id} not found");
        }

        return Ok(equipment);
    }

    /// <summary>
    /// Create new brewing equipment
    /// </summary>
    /// <param name="createDto">Brewing equipment creation data</param>
    /// <returns>Created brewing equipment</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BrewingEquipmentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(201, "BrewingEquipmentSuccess", "Successfully created brewing equipment")]
    [SwaggerResponseExample(400, "ValidationError", "Validation errors in request data")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<BrewingEquipmentResponseDto>> CreateBrewingEquipment([FromBody] CreateBrewingEquipmentDto createDto)
    {
        _logger.LogInformation("Creating new brewing equipment: {Vendor} {Model} ({Type})", 
            createDto.Vendor, createDto.Model, createDto.Type);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdEquipment = await _brewingEquipmentService.CreateAsync(createDto);
        
        _logger.LogInformation("Successfully created brewing equipment with ID: {Id}", createdEquipment.Id);
        
        return CreatedAtAction(
            nameof(GetBrewingEquipment), 
            new { id = createdEquipment.Id }, 
            createdEquipment);
    }

    /// <summary>
    /// Update existing brewing equipment
    /// </summary>
    /// <param name="id">Brewing equipment ID</param>
    /// <param name="updateDto">Brewing equipment update data</param>
    /// <returns>Updated brewing equipment</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BrewingEquipmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "BrewingEquipmentSuccess", "Successfully updated brewing equipment")]
    [SwaggerResponseExample(400, "ValidationError", "Validation errors in request data")]
    [SwaggerResponseExample(404, "NotFoundError", "Brewing equipment not found")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<BrewingEquipmentResponseDto>> UpdateBrewingEquipment(int id, [FromBody] UpdateBrewingEquipmentDto updateDto)
    {
        _logger.LogInformation("Updating brewing equipment with ID: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedEquipment = await _brewingEquipmentService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Successfully updated brewing equipment with ID: {Id}", id);
            return Ok(updatedEquipment);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Brewing equipment with ID {Id} not found for update: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete brewing equipment
    /// </summary>
    /// <param name="id">Brewing equipment ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(404, "NotFoundError", "Brewing equipment not found")]
    [SwaggerResponseExample(409, "ConflictError", "Cannot delete equipment due to existing references")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<IActionResult> DeleteBrewingEquipment(int id)
    {
        _logger.LogInformation("Deleting brewing equipment with ID: {Id}", id);

        try
        {
            await _brewingEquipmentService.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted brewing equipment with ID: {Id}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Brewing equipment with ID {Id} not found for deletion: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Cannot delete brewing equipment with ID {Id}: {Message}", id, ex.Message);
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Get most used brewing equipment ordered by usage frequency (most used first)
    /// </summary>
    /// <param name="count">Number of most used equipment to return. Must be between 1 and 100. Default: 10</param>
    /// <returns>List of most used brewing equipment ordered by usage frequency descending</returns>
    [HttpGet("most-used")]
    [ProducesResponseType(typeof(IEnumerable<BrewingEquipmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "BrewingEquipmentCollection", "Collection of most used brewing equipment")]
    [SwaggerResponseExample(200, "BrewingEquipmentEmpty", "Empty collection when no usage data exists")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<IEnumerable<BrewingEquipmentResponseDto>>> GetMostUsedBrewingEquipment([FromQuery] int count = 10)
    {
        _logger.LogInformation("Getting {Count} most used brewing equipment", count);

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var mostUsedEquipment = await _brewingEquipmentService.GetMostUsedAsync(count);
        return Ok(mostUsedEquipment);
    }

    /// <summary>
    /// Get distinct equipment vendors
    /// </summary>
    /// <returns>List of distinct vendors</returns>
    [HttpGet("vendors")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "VendorsCollection", "Collection of distinct equipment vendors")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<IEnumerable<string>>> GetDistinctVendors()
    {
        _logger.LogInformation("Getting distinct equipment vendors");

        var vendors = await _brewingEquipmentService.GetDistinctVendorsAsync();
        return Ok(vendors);
    }

    /// <summary>
    /// Get distinct equipment models
    /// </summary>
    /// <returns>List of distinct models</returns>
    [HttpGet("models")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(200, "ModelsCollection", "Collection of distinct equipment models")]
    [SwaggerResponseExample(500, "InternalServerError", "Internal server error")]
    public async Task<ActionResult<IEnumerable<string>>> GetDistinctModels()
    {
        _logger.LogInformation("Getting distinct equipment models");

        var models = await _brewingEquipmentService.GetDistinctModelsAsync();
        return Ok(models);
    }
}