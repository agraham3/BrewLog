using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

/// <summary>
/// Represents a coffee bean variety with its characteristics and metadata.
/// This is the core entity that defines the coffee being used in brewing sessions.
/// </summary>
public class CoffeeBean
{
    /// <summary>
    /// Unique identifier for the coffee bean record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name or variety of the coffee bean (e.g., "Ethiopian Yirgacheffe", "Colombian Supremo").
    /// Required field with maximum length of 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The brand or roaster name that produced this coffee bean.
    /// Required field with maximum length of 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// The roast level of the coffee bean, indicating the degree of roasting applied.
    /// This affects the flavor profile, acidity, and brewing characteristics.
    /// </summary>
    public RoastLevel RoastLevel { get; set; }

    /// <summary>
    /// The geographical origin or region where the coffee beans were grown (e.g., "Ethiopia", "Colombia").
    /// Optional field with maximum length of 100 characters.
    /// </summary>
    [MaxLength(100)]
    public string Origin { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when this coffee bean record was created in the system.
    /// Automatically set to UTC time when the record is created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The date and time when this coffee bean record was last modified.
    /// Null if the record has never been updated since creation.
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Navigation property representing all brewing sessions that used this coffee bean.
    /// This establishes a one-to-many relationship where one coffee bean can be used in multiple brew sessions.
    /// </summary>
    public ICollection<BrewSession> BrewSessions { get; set; } = [];
}