using System.ComponentModel.DataAnnotations;

namespace BrewLog.Api.Models;

public class GrindSetting
{
    public int Id { get; set; }
    
    [Range(1, 30)]
    public int GrindSize { get; set; }
    
    public TimeSpan GrindTime { get; set; }
    
    [Range(0.1, 1000.0)]
    public decimal GrindWeight { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string GrinderType { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<BrewSession> BrewSessions { get; set; } = new List<BrewSession>();
}