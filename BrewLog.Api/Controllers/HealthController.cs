using Microsoft.AspNetCore.Mvc;
using BrewLog.Api.DTOs;

namespace BrewLog.Api.Controllers;

/// <summary>
/// Health check controller for monitoring API availability and status
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Performs a health check on the API service
    /// </summary>
    /// <returns>Health status information including service status, timestamp, and version</returns>
    /// <response code="200">Returns the health status of the API service</response>
    /// <example>
    /// GET /api/health
    /// 
    /// Response:
    /// {
    ///   "status": "Healthy",
    ///   "timestamp": "2024-01-15T10:30:00.000Z",
    ///   "version": "1.0.0"
    /// }
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponseDto), StatusCodes.Status200OK)]
    public ActionResult<HealthResponseDto> Get()
    {
        var response = new HealthResponseDto
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        return Ok(response);
    }
}