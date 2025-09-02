using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

/// <summary>
/// Represents the grind parameters used for preparing coffee beans before brewing.
/// This entity captures the critical grind characteristics that significantly impact extraction
/// and final brew quality, including size, timing, weight, and grinder specifications.
/// </summary>
public class GrindSetting
{
    /// <summary>
    /// Unique identifier for the grind setting record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The grind size setting on a standardized scale from 1 to 30.
    /// Lower numbers represent finer grinds (1-10: espresso to medium-fine),
    /// higher numbers represent coarser grinds (20-30: French press to cold brew).
    /// This setting directly affects extraction rate and brewing time requirements.
    /// </summary>
    [Range(1, 30)]
    public int GrindSize { get; set; }

    /// <summary>
    /// The duration of time spent grinding the coffee beans.
    /// Format: HH:MM:SS (e.g., 00:00:15 for 15 seconds).
    /// Grinding time affects particle size consistency and can impact extraction uniformity.
    /// </summary>
    public TimeSpan GrindTime { get; set; }

    /// <summary>
    /// The weight of coffee beans ground for this setting, measured in grams.
    /// Must be between 0.1g and 1000.0g. This measurement is crucial for maintaining
    /// consistent coffee-to-water ratios and reproducible brewing results.
    /// </summary>
    [Range(0.1, 1000.0)]
    public decimal GrindWeight { get; set; }

    /// <summary>
    /// The type or model of grinder used (e.g., "Burr Grinder", "Blade Grinder", "Hand Grinder").
    /// Required field with maximum length of 50 characters.
    /// Different grinder types produce different particle size distributions affecting extraction.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string GrinderType { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes about the grind setting, observations, or special considerations.
    /// Can include details about grind consistency, adjustments made, or environmental factors.
    /// Maximum length of 500 characters.
    /// </summary>
    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when this grind setting record was created in the system.
    /// Automatically set to UTC time when the record is created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property representing all brewing sessions that used this grind setting.
    /// This establishes a one-to-many relationship where one grind setting can be reused across multiple brew sessions.
    /// Useful for tracking the effectiveness of specific grind parameters across different brewing attempts.
    /// </summary>
    public ICollection<BrewSession> BrewSessions { get; set; } = [];
}