using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

/// <summary>
/// Represents a brew session entity in API responses with all properties and related data
/// </summary>
public class BrewSessionResponseDto
{
    /// <summary>
    /// Unique identifier for the brew session
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Brewing method used for this session
    /// </summary>
    /// <example>PourOver</example>
    public BrewMethod Method { get; set; }

    /// <summary>
    /// Water temperature in Celsius (60.0 - 100.0)
    /// </summary>
    /// <example>92.5</example>
    public decimal WaterTemperature { get; set; }

    /// <summary>
    /// Total brewing time in HH:MM:SS format
    /// </summary>
    /// <example>00:04:30</example>
    public TimeSpan BrewTime { get; set; }

    /// <summary>
    /// Tasting notes and flavor descriptions (max 1000 characters)
    /// </summary>
    /// <example>Bright acidity with notes of citrus and chocolate. Clean finish.</example>
    public string TastingNotes { get; set; } = string.Empty;

    /// <summary>
    /// Quality rating from 1 to 10 (optional)
    /// </summary>
    /// <example>8</example>
    public int? Rating { get; set; }

    /// <summary>
    /// Whether this brew session is marked as a favorite
    /// </summary>
    /// <example>true</example>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Date and time when the brew session was created (UTC)
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Identifier of the coffee bean used in this session
    /// </summary>
    /// <example>1</example>
    public int CoffeeBeanId { get; set; }

    /// <summary>
    /// Identifier of the grind setting used in this session
    /// </summary>
    /// <example>1</example>
    public int GrindSettingId { get; set; }

    /// <summary>
    /// Identifier of the brewing equipment used (optional)
    /// </summary>
    /// <example>1</example>
    public int? BrewingEquipmentId { get; set; }

    /// <summary>
    /// Coffee bean details used in this brew session
    /// </summary>
    public CoffeeBeanResponseDto CoffeeBean { get; set; } = null!;

    /// <summary>
    /// Grind setting details used in this brew session
    /// </summary>
    public GrindSettingResponseDto GrindSetting { get; set; } = null!;

    /// <summary>
    /// Brewing equipment details used in this session (if specified)
    /// </summary>
    public BrewingEquipmentResponseDto? BrewingEquipment { get; set; }
}

/// <summary>
/// Data transfer object for creating a new brew session record
/// </summary>
public class CreateBrewSessionDto
{
    /// <summary>
    /// Brewing method to use for this session. Accepted values: Espresso, FrenchPress, PourOver, Drip, AeroPress, ColdBrew
    /// </summary>
    /// <example>PourOver</example>
    public BrewMethod Method { get; set; }

    /// <summary>
    /// Water temperature in Celsius (required, range: 60.0 - 100.0)
    /// </summary>
    /// <example>92.5</example>
    public decimal WaterTemperature { get; set; }

    /// <summary>
    /// Total brewing time in HH:MM:SS format (required)
    /// </summary>
    /// <example>00:04:30</example>
    public TimeSpan BrewTime { get; set; }

    /// <summary>
    /// Tasting notes and flavor descriptions (optional, max 1000 characters)
    /// </summary>
    /// <example>Bright acidity with notes of citrus and chocolate. Clean finish.</example>
    public string TastingNotes { get; set; } = string.Empty;

    /// <summary>
    /// Quality rating from 1 to 10 (optional, range: 1-10)
    /// </summary>
    /// <example>8</example>
    public int? Rating { get; set; }

    /// <summary>
    /// Whether to mark this brew session as a favorite (default: false)
    /// </summary>
    /// <example>true</example>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Identifier of the coffee bean to use (required, must exist)
    /// </summary>
    /// <example>1</example>
    public int CoffeeBeanId { get; set; }

    /// <summary>
    /// Identifier of the grind setting to use (required, must exist)
    /// </summary>
    /// <example>1</example>
    public int GrindSettingId { get; set; }

    /// <summary>
    /// Identifier of the brewing equipment to use (optional, must exist if specified)
    /// </summary>
    /// <example>1</example>
    public int? BrewingEquipmentId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing brew session record
/// </summary>
public class UpdateBrewSessionDto
{
    /// <summary>
    /// Brewing method to use for this session. Accepted values: Espresso, FrenchPress, PourOver, Drip, AeroPress, ColdBrew
    /// </summary>
    /// <example>PourOver</example>
    public BrewMethod Method { get; set; }

    /// <summary>
    /// Water temperature in Celsius (required, range: 60.0 - 100.0)
    /// </summary>
    /// <example>92.5</example>
    public decimal WaterTemperature { get; set; }

    /// <summary>
    /// Total brewing time in HH:MM:SS format (required)
    /// </summary>
    /// <example>00:04:30</example>
    public TimeSpan BrewTime { get; set; }

    /// <summary>
    /// Tasting notes and flavor descriptions (optional, max 1000 characters)
    /// </summary>
    /// <example>Bright acidity with notes of citrus and chocolate. Clean finish.</example>
    public string TastingNotes { get; set; } = string.Empty;

    /// <summary>
    /// Quality rating from 1 to 10 (optional, range: 1-10)
    /// </summary>
    /// <example>8</example>
    public int? Rating { get; set; }

    /// <summary>
    /// Whether to mark this brew session as a favorite
    /// </summary>
    /// <example>true</example>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Identifier of the coffee bean to use (required, must exist)
    /// </summary>
    /// <example>1</example>
    public int CoffeeBeanId { get; set; }

    /// <summary>
    /// Identifier of the grind setting to use (required, must exist)
    /// </summary>
    /// <example>1</example>
    public int GrindSettingId { get; set; }

    /// <summary>
    /// Identifier of the brewing equipment to use (optional, must exist if specified)
    /// </summary>
    /// <example>1</example>
    public int? BrewingEquipmentId { get; set; }
}

/// <summary>
/// Data transfer object for filtering brew session queries with optional search criteria
/// </summary>
public class BrewSessionFilterDto
{
    /// <summary>
    /// Filter by brewing method. Accepted values: Espresso, FrenchPress, PourOver, Drip, AeroPress, ColdBrew
    /// </summary>
    /// <example>PourOver</example>
    public BrewMethod? Method { get; set; }

    /// <summary>
    /// Filter by specific coffee bean ID
    /// </summary>
    /// <example>1</example>
    public int? CoffeeBeanId { get; set; }

    /// <summary>
    /// Filter by specific grind setting ID
    /// </summary>
    /// <example>1</example>
    public int? GrindSettingId { get; set; }

    /// <summary>
    /// Filter by specific brewing equipment ID
    /// </summary>
    /// <example>1</example>
    public int? BrewingEquipmentId { get; set; }

    /// <summary>
    /// Filter for sessions with water temperature at or above this value (Celsius)
    /// </summary>
    /// <example>90.0</example>
    public decimal? MinWaterTemperature { get; set; }

    /// <summary>
    /// Filter for sessions with water temperature at or below this value (Celsius)
    /// </summary>
    /// <example>95.0</example>
    public decimal? MaxWaterTemperature { get; set; }

    /// <summary>
    /// Filter for sessions with rating at or above this value (1-10)
    /// </summary>
    /// <example>7</example>
    public int? MinRating { get; set; }

    /// <summary>
    /// Filter for sessions with rating at or below this value (1-10)
    /// </summary>
    /// <example>10</example>
    public int? MaxRating { get; set; }

    /// <summary>
    /// Filter by favorite status (true for favorites only, false for non-favorites only)
    /// </summary>
    /// <example>true</example>
    public bool? IsFavorite { get; set; }

    /// <summary>
    /// Filter for sessions created after this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-01-01T00:00:00Z</example>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filter for sessions created before this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-12-31T23:59:59Z</example>
    public DateTime? CreatedBefore { get; set; }
}