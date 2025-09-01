using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

public class CoffeeBean
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    public RoastLevel RoastLevel { get; set; }

    [MaxLength(100)]
    public string Origin { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public ICollection<BrewSession> BrewSessions { get; set; } = [];
}