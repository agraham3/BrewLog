using BrewLog.Api.Models;

namespace BrewLog.Api.DTOs;

/// <summary>
/// Represents a coffee bean entity in API responses with all properties and metadata
/// </summary>
public class CoffeeBeanResponseDto
{
    /// <summary>
    /// Unique identifier for the coffee bean
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Name of the coffee bean variety or blend (required, max 100 characters)
    /// </summary>
    /// <example>Ethiopian Yirgacheffe</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Brand or roaster name (required, max 100 characters)
    /// </summary>
    /// <example>Blue Bottle Coffee</example>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Roast level of the coffee beans
    /// </summary>
    /// <example>Medium</example>
    public RoastLevel RoastLevel { get; set; }

    /// <summary>
    /// Geographic origin of the coffee beans (max 100 characters)
    /// </summary>
    /// <example>Ethiopia, Yirgacheffe</example>
    public string Origin { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the coffee bean record was created (UTC)
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Date and time when the coffee bean record was last modified (UTC), null if never modified
    /// </summary>
    /// <example>2024-01-16T14:20:00Z</example>
    public DateTime? ModifiedDate { get; set; }
}

/// <summary>
/// Data transfer object for creating a new coffee bean record
/// </summary>
public class CreateCoffeeBeanDto
{
    /// <summary>
    /// Name of the coffee bean variety or blend (required, max 100 characters)
    /// </summary>
    /// <example>Ethiopian Yirgacheffe</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Brand or roaster name (required, max 100 characters)
    /// </summary>
    /// <example>Blue Bottle Coffee</example>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Roast level of the coffee beans. Accepted values: Light, MediumLight, Medium, MediumDark, Dark
    /// </summary>
    /// <example>Medium</example>
    public RoastLevel RoastLevel { get; set; }

    /// <summary>
    /// Geographic origin of the coffee beans (optional, max 100 characters)
    /// </summary>
    /// <example>Ethiopia, Yirgacheffe</example>
    public string Origin { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating an existing coffee bean record
/// </summary>
public class UpdateCoffeeBeanDto
{
    /// <summary>
    /// Name of the coffee bean variety or blend (required, max 100 characters)
    /// </summary>
    /// <example>Ethiopian Yirgacheffe</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Brand or roaster name (required, max 100 characters)
    /// </summary>
    /// <example>Blue Bottle Coffee</example>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Roast level of the coffee beans. Accepted values: Light, MediumLight, Medium, MediumDark, Dark
    /// </summary>
    /// <example>Medium</example>
    public RoastLevel RoastLevel { get; set; }

    /// <summary>
    /// Geographic origin of the coffee beans (optional, max 100 characters)
    /// </summary>
    /// <example>Ethiopia, Yirgacheffe</example>
    public string Origin { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for filtering coffee bean queries with optional search criteria
/// </summary>
public class CoffeeBeanFilterDto
{
    /// <summary>
    /// Filter by coffee bean name (partial match, case-insensitive)
    /// </summary>
    /// <example>Yirgacheffe</example>
    public string? Name { get; set; }

    /// <summary>
    /// Filter by brand name (partial match, case-insensitive)
    /// </summary>
    /// <example>Blue Bottle</example>
    public string? Brand { get; set; }

    /// <summary>
    /// Filter by specific roast level. Accepted values: Light, MediumLight, Medium, MediumDark, Dark
    /// </summary>
    /// <example>Medium</example>
    public RoastLevel? RoastLevel { get; set; }

    /// <summary>
    /// Filter by origin (partial match, case-insensitive)
    /// </summary>
    /// <example>Ethiopia</example>
    public string? Origin { get; set; }

    /// <summary>
    /// Filter for coffee beans created after this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-01-01T00:00:00Z</example>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filter for coffee beans created before this date (inclusive, ISO 8601 format)
    /// </summary>
    /// <example>2024-12-31T23:59:59Z</example>
    public DateTime? CreatedBefore { get; set; }
}