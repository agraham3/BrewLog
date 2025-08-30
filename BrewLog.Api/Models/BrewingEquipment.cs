using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

public class BrewingEquipment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Vendor { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    public EquipmentType Type { get; set; }

    public Dictionary<string, string> Specifications { get; set; } = [];

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<BrewSession> BrewSessions { get; set; } = [];
}