using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

/// <summary>
/// Represents brewing equipment entity in API responses with all properties and specifications
/// </summary>
public class BrewingEquipmentResponseDto
{
    /// <summary>
    /// Unique identifier for the brewing equipment
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Equipment vendor or manufacturer name (required, max 100 characters)
    /// </summary>
    /// <example>Hario</example>
    public string Vendor { get; set; } = string.Empty;

    /// <summary>
    /// Equipment model name or number (required, max 100 characters)
    /// </summary>
    /// <example>V60-02</example>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Type of brewing equipment
    /// </summary>
    /// <example>PourOverSetup</example>
    public EquipmentType Type { get; set; }

    /// <summary>
    /// Equipment specifications as key-value pairs. Common keys include: "capacity", "material", "dimensions", "weight", "power", "pressure", "temperature_range"
    /// </summary>
    /// <example>
    /// {
    ///   "capacity": "1-4 cups",
    ///   "material": "Ceramic",
    ///   "dimensions": "11.6 x 10 x 8.2 cm",
    ///   "weight": "340g"
    /// }
    /// </example>
    public Dictionary<string, string> Specifications { get; set; } = [];

    /// <summary>
    /// Date and time when the equipment record was created (UTC)
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Data transfer object for creating a new brewing equipment record
/// </summary>
public class CreateBrewingEquipmentDto
{
    /// <summary>
    /// Equipment vendor or manufacturer name (required, max 100 characters)
    /// </summary>
    /// <example>Hario</example>
    public string Vendor { get; set; } = string.Empty;

    /// <summary>
    /// Equipment model name or number (required, max 100 characters)
    /// </summary>
    /// <example>V60-02</example>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Type of brewing equipment. Accepted values: EspressoMachine, Grinder, FrenchPress, PourOverSetup, DripMachine, AeroPress
    /// </summary>
    /// <example>PourOverSetup</example>
    public EquipmentType Type { get; set; }

    /// <summary>
    /// Equipment specifications as key-value pairs. Common keys include: "capacity", "material", "dimensions", "weight", "power", "pressure", "temperature_range". All values should be strings.
    /// </summary>
    /// <example>
    /// {
    ///   "capacity": "1-4 cups",
    ///   "material": "Ceramic",
    ///   "dimensions": "11.6 x 10 x 8.2 cm",
    ///   "weight": "340g"
    /// }
    /// </example>
    public Dictionary<string, string> Specifications { get; set; } = [];
}

/// <summary>
/// Data transfer object for updating an existing brewing equipment record
/// </summary>
public class UpdateBrewingEquipmentDto
{
    /// <summary>
    /// Equipment vendor or manufacturer name (required, max 100 characters)
    /// </summary>
    /// <example>Hario</example>
    public string Vendor { get; set; } = string.Empty;

    /// <summary>
    /// Equipment model name or number (required, max 100 characters)
    /// </summary>
    /// <example>V60-02</example>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Type of brewing equipment. Accepted values: EspressoMachine, Grinder, FrenchPress, PourOverSetup, DripMachine, AeroPress
    /// </summary>
    /// <example>PourOverSetup</example>
    public EquipmentType Type { get; set; }

    /// <summary>
    /// Equipment specifications as key-value pairs. Common keys include: "capacity", "material", "dimensions", "weight", "power", "pressure", "temperature_range". All values should be strings.
    /// </summary>
    /// <example>
    /// {
    ///   "capacity": "1-4 cups",
    ///   "material": "Ceramic",
    ///   "dimensions": "11.6 x 10 x 8.2 cm",
    ///   "weight": "340g"
    /// }
    /// </example>
    public Dictionary<string, string> Specifications { get; set; } = [];
}

/// <summary>
/// Data transfer object for filtering brewing equipment queries with optional search criteria
/// </summary>
public class BrewingEquipmentFilterDto
{
    /// <summary>
    /// Filter by vendor name (partial match, case-insensitive)
    /// </summary>
    /// <example>Hario</example>
    public string? Vendor { get; set; }

    /// <summary>
    /// Filter by model name (partial match, case-insensitive)
    /// </summary>
    /// <example>V60</example>
    public string? Model { get; set; }

    /// <summary>
    /// Filter by equipment type. Accepted values: EspressoMachine, Grinder, FrenchPress, PourOverSetup, DripMachine, AeroPress
    /// </summary>
    /// <example>PourOverSetup</example>
    public EquipmentType? Type { get; set; }

    /// <summary>
    /// Filter for equipment created after this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-01-01T00:00:00Z</example>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filter for equipment created before this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-12-31T23:59:59Z</example>
    public DateTime? CreatedBefore { get; set; }
}