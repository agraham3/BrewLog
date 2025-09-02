using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

/// <summary>
/// Represents brewing equipment used in coffee preparation sessions.
/// This entity captures the specific tools, machines, and devices used for brewing,
/// including their specifications and technical details that may affect brewing outcomes.
/// </summary>
public class BrewingEquipment
{
    /// <summary>
    /// Unique identifier for the brewing equipment record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The manufacturer or brand name of the brewing equipment (e.g., "Hario", "Chemex", "Breville").
    /// Required field with maximum length of 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Vendor { get; set; } = string.Empty;

    /// <summary>
    /// The specific model name or number of the brewing equipment (e.g., "V60-02", "Classic Series", "Barista Express").
    /// Required field with maximum length of 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The category or type of brewing equipment, which determines its primary function and brewing method compatibility.
    /// This helps categorize equipment for filtering and selection purposes.
    /// </summary>
    public EquipmentType Type { get; set; }

    /// <summary>
    /// A flexible dictionary containing technical specifications and characteristics of the equipment.
    /// Key-value pairs can include details like "Capacity: 350ml", "Material: Ceramic", "Filter Type: Paper", etc.
    /// This allows for equipment-specific details without rigid schema constraints.
    /// </summary>
    public Dictionary<string, string> Specifications { get; set; } = [];

    /// <summary>
    /// The date and time when this brewing equipment record was created in the system.
    /// Automatically set to UTC time when the record is created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property representing all brewing sessions that used this equipment.
    /// This establishes a one-to-many relationship where one piece of equipment can be used in multiple brew sessions.
    /// Useful for tracking equipment usage patterns and performance correlations.
    /// </summary>
    public ICollection<BrewSession> BrewSessions { get; set; } = [];
}