namespace BrewLog.Api.DTOs;

/// <summary>
/// Response object for the health check endpoint, providing API status and metadata
/// </summary>
public class HealthResponseDto
{
    /// <summary>
    /// Current status of the API service
    /// </summary>
    /// <example>Healthy</example>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the health check was performed
    /// </summary>
    /// <example>2024-01-15T10:30:00.000Z</example>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Current version of the API
    /// </summary>
    /// <example>1.0.0</example>
    public string Version { get; set; } = string.Empty;
}