using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

/// <summary>
/// Represents a single coffee brewing session with all parameters, results, and relationships.
/// This is the central entity that captures the complete brewing experience including method,
/// parameters, sensory evaluation, and references to the coffee bean, grind setting, and equipment used.
/// </summary>
public class BrewSession
{
    /// <summary>
    /// Unique identifier for the brewing session record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The brewing method used for this session (e.g., Espresso, Pour Over, French Press).
    /// This determines the extraction technique and influences other brewing parameters.
    /// </summary>
    public BrewMethod Method { get; set; }

    /// <summary>
    /// Water temperature used during brewing, measured in Celsius.
    /// Must be between 60.0°C and 100.0°C. Different brewing methods typically require different temperature ranges
    /// (e.g., espresso: 90-96°C, pour over: 85-95°C, cold brew: room temperature).
    /// </summary>
    [Range(60.0, 100.0)]
    public decimal WaterTemperature { get; set; }

    /// <summary>
    /// Total time spent brewing the coffee, from start to finish.
    /// Format: HH:MM:SS (e.g., 00:04:30 for 4 minutes and 30 seconds).
    /// Brewing time varies significantly by method (espresso: 25-30s, pour over: 3-6 minutes).
    /// </summary>
    public TimeSpan BrewTime { get; set; }

    /// <summary>
    /// Detailed tasting notes and sensory evaluation of the brewed coffee.
    /// Can include flavor descriptors, aroma notes, body characteristics, and overall impressions.
    /// Maximum length of 1000 characters.
    /// </summary>
    [MaxLength(1000)]
    public string TastingNotes { get; set; } = string.Empty;

    /// <summary>
    /// Subjective quality rating of the brewing session on a scale of 1-10.
    /// 1 represents poor quality, 10 represents exceptional quality.
    /// Optional field - null if no rating was provided.
    /// </summary>
    [Range(1, 10)]
    public int? Rating { get; set; }

    /// <summary>
    /// Indicates whether this brewing session is marked as a favorite.
    /// Useful for quickly identifying successful brewing combinations for future reference.
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// The date and time when this brewing session was recorded in the system.
    /// Automatically set to UTC time when the record is created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Foreign key reference to the coffee bean used in this brewing session.
    /// Required - every brewing session must be associated with a specific coffee bean.
    /// </summary>
    public int CoffeeBeanId { get; set; }

    /// <summary>
    /// Foreign key reference to the grind setting used in this brewing session.
    /// Required - every brewing session must specify the grind parameters used.
    /// </summary>
    public int GrindSettingId { get; set; }

    /// <summary>
    /// Foreign key reference to the brewing equipment used in this session.
    /// Optional - null if no specific equipment was recorded or if using basic equipment.
    /// </summary>
    public int? BrewingEquipmentId { get; set; }

    /// <summary>
    /// Navigation property to the coffee bean entity used in this brewing session.
    /// Provides access to all coffee bean details including name, brand, roast level, and origin.
    /// </summary>
    public CoffeeBean CoffeeBean { get; set; } = null!;

    /// <summary>
    /// Navigation property to the grind setting entity used in this brewing session.
    /// Provides access to grind size, time, weight, and grinder type information.
    /// </summary>
    public GrindSetting GrindSetting { get; set; } = null!;

    /// <summary>
    /// Navigation property to the brewing equipment entity used in this session.
    /// Null if no specific equipment was used. Provides access to equipment specifications and details.
    /// </summary>
    public BrewingEquipment? BrewingEquipment { get; set; }
}