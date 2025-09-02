namespace BrewLog.Api.DTOs;

/// <summary>
/// Represents a grind setting entity in API responses with all properties and metadata
/// </summary>
public class GrindSettingResponseDto
{
    /// <summary>
    /// Unique identifier for the grind setting
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Grind size setting on a scale from 1 (finest) to 30 (coarsest)
    /// </summary>
    /// <example>15</example>
    public int GrindSize { get; set; }

    /// <summary>
    /// Duration of grinding in HH:MM:SS format
    /// </summary>
    /// <example>00:00:20</example>
    public TimeSpan GrindTime { get; set; }

    /// <summary>
    /// Weight of coffee beans to grind in grams (0.1 - 1000.0)
    /// </summary>
    /// <example>22.5</example>
    public decimal GrindWeight { get; set; }

    /// <summary>
    /// Type or model of grinder used (required, max 50 characters)
    /// </summary>
    /// <example>Baratza Encore</example>
    public string GrinderType { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes about the grind setting (max 500 characters)
    /// </summary>
    /// <example>Medium grind for pour over, consistent particle size</example>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the grind setting was created (UTC)
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Data transfer object for creating a new grind setting record
/// </summary>
public class CreateGrindSettingDto
{
    /// <summary>
    /// Grind size setting on a scale from 1 (finest) to 30 (coarsest) - required, range: 1-30
    /// </summary>
    /// <example>15</example>
    public int GrindSize { get; set; }

    /// <summary>
    /// Duration of grinding in HH:MM:SS format (required)
    /// </summary>
    /// <example>00:00:20</example>
    public TimeSpan GrindTime { get; set; }

    /// <summary>
    /// Weight of coffee beans to grind in grams (required, range: 0.1 - 1000.0)
    /// </summary>
    /// <example>22.5</example>
    public decimal GrindWeight { get; set; }

    /// <summary>
    /// Type or model of grinder used (required, max 50 characters)
    /// </summary>
    /// <example>Baratza Encore</example>
    public string GrinderType { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes about the grind setting (optional, max 500 characters)
    /// </summary>
    /// <example>Medium grind for pour over, consistent particle size</example>
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating an existing grind setting record
/// </summary>
public class UpdateGrindSettingDto
{
    /// <summary>
    /// Grind size setting on a scale from 1 (finest) to 30 (coarsest) - required, range: 1-30
    /// </summary>
    /// <example>15</example>
    public int GrindSize { get; set; }

    /// <summary>
    /// Duration of grinding in HH:MM:SS format (required)
    /// </summary>
    /// <example>00:00:20</example>
    public TimeSpan GrindTime { get; set; }

    /// <summary>
    /// Weight of coffee beans to grind in grams (required, range: 0.1 - 1000.0)
    /// </summary>
    /// <example>22.5</example>
    public decimal GrindWeight { get; set; }

    /// <summary>
    /// Type or model of grinder used (required, max 50 characters)
    /// </summary>
    /// <example>Baratza Encore</example>
    public string GrinderType { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes about the grind setting (optional, max 500 characters)
    /// </summary>
    /// <example>Medium grind for pour over, consistent particle size</example>
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for filtering grind setting queries with optional search criteria
/// </summary>
public class GrindSettingFilterDto
{
    /// <summary>
    /// Filter for grind settings with size at or above this value (1-30 scale)
    /// </summary>
    /// <example>10</example>
    public int? MinGrindSize { get; set; }

    /// <summary>
    /// Filter for grind settings with size at or below this value (1-30 scale)
    /// </summary>
    /// <example>20</example>
    public int? MaxGrindSize { get; set; }

    /// <summary>
    /// Filter by grinder type (partial match, case-insensitive)
    /// </summary>
    /// <example>Baratza</example>
    public string? GrinderType { get; set; }

    /// <summary>
    /// Filter for grind settings with weight at or above this value (grams)
    /// </summary>
    /// <example>20.0</example>
    public decimal? MinGrindWeight { get; set; }

    /// <summary>
    /// Filter for grind settings with weight at or below this value (grams)
    /// </summary>
    /// <example>30.0</example>
    public decimal? MaxGrindWeight { get; set; }

    /// <summary>
    /// Filter for grind settings created after this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-01-01T00:00:00Z</example>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filter for grind settings created before this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-12-31T23:59:59Z</example>
    public DateTime? CreatedBefore { get; set; }
}